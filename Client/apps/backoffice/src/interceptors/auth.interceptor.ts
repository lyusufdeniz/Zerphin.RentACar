import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Skip interceptor for login/register/refresh-token endpoints
  if (
    req.url.includes('/Authentication/login') ||
    req.url.includes('/Authentication/register') ||
    req.url.includes('/Authentication/refresh-token')
  ) {
    return next(req);
  }

  // Check if token is expired or expiring soon
  if (authService.isTokenExpired()) {
    const refreshToken = authService.refreshTokenValue;

    // If refresh token exists, try to refresh
    if (refreshToken) {
      return authService.refreshToken().pipe(
        switchMap(() => {
          // Retry original request with new token
          const token = authService.token;
          if (token) {
            const clonedReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${token}`,
              },
            });
            return next(clonedReq);
          }
          // If token refresh failed, redirect to login
          authService.logout();
          router.navigate(['/login']);
          return throwError(() => new Error('Token refresh failed'));
        }),
        catchError((error) => {
          // Refresh token is invalid or expired
          console.error('Token refresh failed in interceptor:', error);
          authService.logout();
          router.navigate(['/login']);
          return throwError(() => error);
        })
      );
    } else {
      // No refresh token, redirect to login
      console.warn('No refresh token available, redirecting to login');
      authService.logout();
      router.navigate(['/login']);
      return throwError(() => new Error('No refresh token available'));
    }
  }

  // Token is valid, add Authorization header if token exists
  const token = authService.token;
  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(clonedReq);
  }

  // No token, proceed without Authorization header
  return next(req);
};

