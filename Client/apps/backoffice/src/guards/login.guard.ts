import { inject } from '@angular/core';
import {
  Router,
  CanActivateFn,
  ActivatedRouteSnapshot,
  RouterStateSnapshot,
} from '@angular/router';
import { AuthService } from '../services/auth.service';

/**
 * Login Guard - Redirects authenticated users away from login page
 * Prevents authenticated users from accessing login page
 */
export const loginGuard: CanActivateFn = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot
): boolean => {
  const router = inject(Router);
  const authService = inject(AuthService);

  // If user is already authenticated, redirect to dashboard
  if (authService.isAuthenticated()) {
    // Get returnUrl from query params if available
    const returnUrl = route.queryParams['returnUrl'] || '/dashboard';
    router.navigate([returnUrl]);
    return false;
  }

  return true;
};


