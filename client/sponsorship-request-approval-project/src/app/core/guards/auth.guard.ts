import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = route => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const requiredRoles = route.data['roles'] as string[] | undefined;

  if (authService.isAuthenticated()) {
    if (!requiredRoles?.length || authService.hasAnyRole(requiredRoles)) {
      return true;
    }

    return router.createUrlTree(['/sponsorship-requests']);
  }

  return router.createUrlTree(['/auth/login'], {
    queryParams: {
      returnUrl: router.url
    }
  });
};
