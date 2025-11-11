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

  private authToken = signal<string | null>(null);
  private refreshTokenSignal = signal<string | null>(null);
  private currentUser = signal<User | null>(null);

  isAuthenticated = computed(() => {
    return this.authToken() !== null && this.currentUser() !== null;
  });

  get user(): User | null {
    return this.currentUser();
  }

  get userFullName(): string {
    const user = this.currentUser();
    if (!user) return '';
    return `${user.firstName} ${user.lastName}`.trim();
  }

  get token(): string | null {
    return this.authToken();
  }

  get refreshTokenValue(): string | null {
    return this.refreshTokenSignal();
  }

  constructor() {

    this.initializeAuth();
  }

  private initializeAuth(): void {

    const rememberMeLocal = this.storage.getItemString('rememberMe');
    const rememberMeSession = this.storage.getItemStringSession('rememberMe');

    let token: string | null = null;
    let refreshToken: string | null = null;
    let userData: LoginUserResponse | null = null;

    if (rememberMeLocal === 'true') {

      token = this.storage.getItemString('authToken');
      refreshToken = this.storage.getItemString('refreshToken');
      userData = this.storage.getItem<LoginUserResponse>('user');
    } else if (rememberMeSession === 'false') {

      token = this.storage.getItemStringSession('authToken');
      refreshToken = this.storage.getItemStringSession('refreshToken');
      userData = this.storage.getItemSession<LoginUserResponse>('user');
    } else {

      token = this.storage.getItemString('authToken') || this.storage.getItemStringSession('authToken');
      refreshToken = this.storage.getItemString('refreshToken') || this.storage.getItemStringSession('refreshToken');
      userData = this.storage.getItem<LoginUserResponse>('user') || this.storage.getItemSession<LoginUserResponse>('user');
    }

    if (token && refreshToken && userData) {
      try {

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

  private getRoleFromName(roleName: string): number {
    const roleMap: Record<string, UserRole> = {
      Customer: UserRole.Customer,
      Employee: UserRole.Employee,
      Manager: UserRole.Manager,
      Admin: UserRole.Admin,
    };
    return roleMap[roleName] || UserRole.Customer;
  }

  login(credentials: LoginRequest, rememberMe: boolean = true): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>('/Authentication/login', credentials)
      .pipe(
        tap((result) => {
          if (result) {

            const loginUser = result.user;
            const user: User = {
              id: loginUser.id,
              firstName: loginUser.firstName,
              lastName: loginUser.lastName,
              email: loginUser.email,
              roleName: loginUser.roleName,
              role: this.getRoleFromName(loginUser.roleName),
            };

            this.setAuth(
              result.token,
              result.refreshToken,
              user,
              result.expiresAt,
              rememberMe
            );

            console.log('Login successful - Tokens saved');
          }
        }),
        catchError((error) => {
          console.error('Login error:', error);
          return throwError(() => error);
        })
      );
  }

  logout(): void {
    this.clearAuth();
    this.router.navigate(['/login']);
  }

  private setAuth(
    token: string,
    refreshToken: string,
    user: User,
    expiresAt: string,
    rememberMe: boolean = true
  ): void {

    this.authToken.set(token);
    this.refreshTokenSignal.set(refreshToken);
    this.currentUser.set(user);

    const loginUserResponse: LoginUserResponse = {
      id: user.id,
      firstName: user.firstName,
      lastName: user.lastName,
      email: user.email,
      roleName: user.roleName,
    };

    this.clearStorage();

    if (rememberMe) {

      this.storage.setItem('authToken', token);
      this.storage.setItem('refreshToken', refreshToken);
      this.storage.setItem('user', loginUserResponse);
      this.storage.setItem('expiresAt', expiresAt);
      this.storage.setItem('rememberMe', 'true');
    } else {

      this.storage.setItemSession('authToken', token);
      this.storage.setItemSession('refreshToken', refreshToken);
      this.storage.setItemSession('user', loginUserResponse);
      this.storage.setItemSession('expiresAt', expiresAt);
      this.storage.setItemSession('rememberMe', 'false');
    }

    console.log('Auth data saved:', {
      rememberMe,
      storage: rememberMe ? 'localStorage' : 'sessionStorage',
      tokenSaved: !!token,
      refreshTokenSaved: !!refreshToken,
      expiresAt,
    });
  }

  private clearStorage(): void {

    this.storage.removeItem('authToken');
    this.storage.removeItem('refreshToken');
    this.storage.removeItem('user');
    this.storage.removeItem('expiresAt');
    this.storage.removeItem('rememberMe');

    this.storage.removeItemSession('authToken');
    this.storage.removeItemSession('refreshToken');
    this.storage.removeItemSession('user');
    this.storage.removeItemSession('expiresAt');
    this.storage.removeItemSession('rememberMe');
  }

  private clearAuth(): void {
    this.authToken.set(null);
    this.refreshTokenSignal.set(null);
    this.currentUser.set(null);
    this.clearStorage();
  }

  hasRole(roleName: string): boolean {
    const user = this.currentUser();
    return user?.roleName === roleName;
  }

  hasAnyRole(roleNames: string[]): boolean {
    const user = this.currentUser();
    return user?.roleName ? roleNames.includes(user.roleName) : false;
  }

  register(registerData: RegisterRequest): Observable<ServiceResult<any>> {
    return this.http.post<ServiceResult<any>>(
      '/Authentication/register',
      registerData
    );
  }

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

              const rememberMe = this.storage.getItemString('rememberMe') === 'true' || 
                                 this.storage.getItemStringSession('rememberMe') === 'false';

              this.setAuth(
                result.token,
                result.refreshToken,
                currentUser,
                result.expiresAt,
                rememberMe
              );
              console.log('Token refreshed successfully - New tokens saved');
            }
          }
        }),
        catchError((error) => {
          console.error('Token refresh error:', error);

          this.clearAuth();
          return throwError(() => error);
        })
      );
  }

  isTokenExpired(): boolean {

    const rememberMe = this.storage.getItemString('rememberMe') === 'true' || 
                       this.storage.getItemStringSession('rememberMe') === 'false';

    const expiresAt = rememberMe
      ? this.storage.getItemString('expiresAt')
      : this.storage.getItemStringSession('expiresAt');

    const expiresAtValue = expiresAt || 
                          this.storage.getItemString('expiresAt') || 
                          this.storage.getItemStringSession('expiresAt');

    if (!expiresAtValue) {
      return true;
    }

    const expiryDate = new Date(expiresAtValue);
    const now = new Date();

    const bufferMinutes = 5;
    const bufferTime = new Date(expiryDate.getTime() - bufferMinutes * 60 * 1000);

    return now >= bufferTime;
  }

  isTokenExpiringSoon(): boolean {

    const rememberMe = this.storage.getItemString('rememberMe') === 'true' || 
                       this.storage.getItemStringSession('rememberMe') === 'false';

    const expiresAt = rememberMe
      ? this.storage.getItemString('expiresAt')
      : this.storage.getItemStringSession('expiresAt');

    const expiresAtValue = expiresAt || 
                          this.storage.getItemString('expiresAt') || 
                          this.storage.getItemStringSession('expiresAt');

    if (!expiresAtValue) {
      return true;
    }

    const expiryDate = new Date(expiresAtValue);
    const now = new Date();
    const bufferMinutes = 5;
    const bufferTime = new Date(expiryDate.getTime() - bufferMinutes * 60 * 1000);

    return now >= bufferTime && now < expiryDate;
  }
}
