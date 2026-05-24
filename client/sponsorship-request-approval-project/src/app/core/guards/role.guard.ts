import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleGuard: CanActivateFn = route => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const requiredRoles = route.data['roles'] as string[] | undefined;
  const requiredAuthorities = route.data['authorities'] as string[] | undefined;

  if (!authService.isAuthenticated()) {
    return router.createUrlTree(['/login']);
  }

  const hasRoleRequirement = !!requiredRoles?.length;
  const hasAuthorityRequirement = !!requiredAuthorities?.length;
  const roleAllowed = hasRoleRequirement && authService.hasAnyRole(requiredRoles ?? []);
  const authorityAllowed = hasAuthorityRequirement && (requiredAuthorities ?? []).some(authority =>
    authService.hasAuthority(authority as 'RequestorAccess' | 'ManagerApproval' | 'FinanceApproval'));

  if ((!hasRoleRequirement && !hasAuthorityRequirement)
    || (hasRoleRequirement && !hasAuthorityRequirement && roleAllowed)
    || (hasAuthorityRequirement && authorityAllowed)) {
    return true;
  }

  const fallback = authService.getLandingRoute();
  return router.createUrlTree([fallback]);
};
