import { Injectable, inject, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { HttpService } from './http.service';
import { StorageService } from './storage.service';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RefreshTokenRequest,
  RefreshTokenResponse,
  LoginUserResponse,
} from '../models/auth.models';
import { ServiceResult } from '../models/api/api-response.model';
import { User } from '../models/user/user.model';
import { UserRole } from '../models/user/user-role.model';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpService);
  private router = inject(Router);
  private storage = inject(StorageService);

  // Signal for authentication state
  private authToken = signal<string | null>(null);
  private refreshTokenSignal = signal<string | null>(null);
  private currentUser = signal<User | null>(null);

  // Computed signal for isAuthenticated
  isAuthenticated = computed(() => {
    return this.authToken() !== null && this.currentUser() !== null;
  });

  // Public getter for current user
  get user(): User | null {
    return this.currentUser();
  }

  // Get user full name
  get userFullName(): string {
    const user = this.currentUser();
    if (!user) return '';
    return `${user.firstName} ${user.lastName}`.trim();
  }

  // Public getter for token
  get token(): string | null {
    return this.authToken();
  }

  get refreshTokenValue(): string | null {
    return this.refreshTokenSignal();
  }

  constructor() {
    // Check if user is already logged in (e.g., from previous session)
    this.initializeAuth();
  }

  /**
   * Initialize authentication from storage
   */
  private initializeAuth(): void {
    const token = this.storage.getItemString('authToken');
    const refreshToken = this.storage.getItemString('refreshToken');
    const userData = this.storage.getItem<LoginUserResponse>('user');

    if (token && refreshToken && userData) {
      try {
        // Convert LoginUserResponse to User
        const user: User = {
          id: userData.id,
          firstName: userData.firstName,
          lastName: userData.lastName,
          email: userData.email,
          roleName: userData.roleName,
          role: this.getRoleFromName(userData.roleName),
        };
        this.authToken.set(token);
        this.refreshTokenSignal.set(refreshToken);
        this.currentUser.set(user);
      } catch (error) {
        console.error('Error parsing user data:', error);
        this.clearAuth();
      }
    }
  }

  /**
   * Convert role name to UserRole enum
   */
  private getRoleFromName(roleName: string): number {
    const roleMap: Record<string, UserRole> = {
      Customer: UserRole.Customer,
      Employee: UserRole.Employee,
      Manager: UserRole.Manager,
      Admin: UserRole.Admin,
    };
    return roleMap[roleName] || UserRole.Customer;
  }

  /**
   * Login user
   */
  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>('/Authentication/login', credentials)
      .pipe(
        tap((result) => {
          if (result) {
            // Convert LoginUserResponse to User
            const loginUser = result.user;
            const user: User = {
              id: loginUser.id,
              firstName: loginUser.firstName,
              lastName: loginUser.lastName,
              email: loginUser.email,
              roleName: loginUser.roleName,
              role: this.getRoleFromName(loginUser.roleName),
            };
            
            // Save tokens and user to localStorage
            this.setAuth(
              result.token,
              result.refreshToken,
              user,
              result.expiresAt
            );

            console.log('Login successful - Tokens saved to localStorage');
          }
        }),
        catchError((error) => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }

  /**
   * Logout user
   */
  logout(): void {
    this.clearAuth();
    this.router.navigate(['/login']);
  }

  /**
   * Set authentication data and save to localStorage
   */
  private setAuth(
    token: string,
    refreshToken: string,
    user: User,
    expiresAt: string
  ): void {
    // Update signals
    this.authToken.set(token);
    this.refreshTokenSignal.set(refreshToken);
    this.currentUser.set(user);

    // Store in localStorage for persistence
    const loginUserResponse: LoginUserResponse = {
      id: user.id,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      roleName: user.roleName,
    };

    // Save tokens and user data to localStorage
    this.storage.setItem('authToken', token);
    this.storage.setItem('refreshToken', refreshToken);
    this.storage.setItem('user', loginUserResponse);
    this.storage.setItem('expiresAt', expiresAt);

    console.log('Auth data saved to localStorage:', {
      tokenSaved: !!token,
      refreshTokenSaved: !!refreshToken,
      expiresAt,
    });
  }

  /**
   * Clear authentication data
   */
  private clearAuth(): void {
    this.authToken.set(null);
    this.refreshTokenSignal.set(null);
    this.currentUser.set(null);
    this.storage.removeItem('authToken');
    this.storage.removeItem('refreshToken');
    this.storage.removeItem('user');
    this.storage.removeItem('expiresAt');
  }

  /**
   * Check if user has specific role
   */
  hasRole(roleName: string): boolean {
    const user = this.currentUser();
    return user?.roleName === roleName;
  }

  /**
   * Check if user has any of the specified roles
   */
  hasAnyRole(roleNames: string[]): boolean {
    const user = this.currentUser();
    return user?.roleName ? roleNames.includes(user.roleName) : false;
  }

  /**
   * Register new user
   */
  register(registerData: RegisterRequest): Observable<ServiceResult<any>> {
    return this.http.post<ServiceResult<any>>(
      '/Authentication/register',
      registerData
    );
  }

  /**
   * Refresh token
   */
  refreshToken(): Observable<RefreshTokenResponse> {
    const refreshTokenValue = this.refreshTokenSignal();
    if (!refreshTokenValue) {
      return throwError(() => new Error('No refresh token available'));
    }

    console.log('Refreshing token...');

    return this.http
      .post<RefreshTokenResponse>(
        '/Authentication/refresh-token',
        { refreshToken: refreshTokenValue } as RefreshTokenRequest
      )
      .pipe(
        tap((result) => {
          if (result) {
            const currentUser = this.currentUser();
            if (currentUser) {
              // Save new tokens to localStorage
              this.setAuth(
                result.token,
                result.refreshToken,
                currentUser,
                result.expiresAt
              );
              console.log('Token refreshed successfully - New tokens saved to localStorage');
            }
          }
        }),
        catchError((error) => {
          console.error('Token refresh error:', error);
          // Clear auth data and redirect to login
          this.clearAuth();
          return throwError(() => error);
        })
      );
  }

  /**
   * Check if token is expired
   */
  isTokenExpired(): boolean {
    const expiresAt = this.storage.getItemString('expiresAt');
    if (!expiresAt) {
      return true;
    }

    const expiryDate = new Date(expiresAt);
    const now = new Date();
    
    // Add 5 minute buffer before actual expiration
    const bufferMinutes = 5;
    const bufferTime = new Date(expiryDate.getTime() - bufferMinutes * 60 * 1000);
    
    return now >= bufferTime;
  }

  /**
   * Check if token will expire soon (within buffer time)
   */
  isTokenExpiringSoon(): boolean {
    const expiresAt = this.storage.getItemString('expiresAt');
    if (!expiresAt) {
      return true;
    }

    const expiryDate = new Date(expiresAt);
    const now = new Date();
    const bufferMinutes = 5;
    const bufferTime = new Date(expiryDate.getTime() - bufferMinutes * 60 * 1000);
    
    return now >= bufferTime && now < expiryDate;
  }
}

