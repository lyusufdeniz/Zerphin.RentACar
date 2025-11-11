import { HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (
    req.url.includes('/Authentication/login') ||
    req.url.includes('/Authentication/register') ||
    req.url.includes('/Authentication/refresh-token')
  ) {
    return next(req);
  }

  if (authService.isTokenExpired()) {
    const refreshToken = authService.refreshTokenValue;

    if (refreshToken) {
      return authService.refreshToken().pipe(
        switchMap(() => {

          const token = authService.token;
          if (token) {
            const clonedReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${token}`,
              },
            });
            return next(clonedReq);
          }

          authService.logout();
          router.navigate(['/login']);
          return throwError(() => new Error('Token refresh failed'));
        }),
        catchError((error) => {

          console.error('Token refresh failed in interceptor:', error);
          authService.logout();
          router.navigate(['/login']);
          return throwError(() => error);
        })
      );
    } else {

      console.warn('No refresh token available, redirecting to login');
      authService.logout();
      router.navigate(['/login']);
      return throwError(() => new Error('No refresh token available'));
    }
  }

  const token = authService.token;
  if (token) {
    const clonedReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`,
      },
    });
    return next(clonedReq);
  }

  return next(req);
};
