import { Injectable } from '@angular/core';

const TOKEN_KEY = 'sponsorship.jwt';
const APPROVAL_STAGE_CLAIM = 'approval_stage';
const ACCESS_SCOPE_CLAIM = 'access_scope';

@Injectable({ providedIn: 'root' })
export class TokenService {
  setToken(token: string): void {
    localStorage.setItem(TOKEN_KEY, token);
  }

  getToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  removeToken(): void {
    localStorage.removeItem(TOKEN_KEY);
  }

  decodeToken(token?: string): Record<string, unknown> | null {
    const raw = token ?? this.getToken();
    if (!raw) {
      return null;
    }

    try {
      const payload = raw.split('.')[1];
      if (!payload) {
        return null;
      }

      return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/'))) as Record<string, unknown>;
    } catch {
      return null;
    }
  }

  getRolesFromToken(): string[] {
    const payload = this.decodeToken();
    if (!payload) {
      return [];
    }

    const roleClaim = payload['role'] ?? payload['roles'] ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if (Array.isArray(roleClaim)) {
      return roleClaim.map(value => String(value));
    }
    if (typeof roleClaim === 'string') {
      return [roleClaim];
    }
    return [];
  }

  getApprovalStagesFromToken(): string[] {
    const payload = this.decodeToken();
    if (!payload) {
      return [];
    }

    const stageClaim = payload[APPROVAL_STAGE_CLAIM];
    if (Array.isArray(stageClaim)) {
      return stageClaim.map(value => String(value));
    }
    if (typeof stageClaim === 'string') {
      return [stageClaim];
    }
    return [];
  }

  getAccessScopesFromToken(): string[] {
    const payload = this.decodeToken();
    if (!payload) {
      return [];
    }

    const scopeClaim = payload[ACCESS_SCOPE_CLAIM];
    if (Array.isArray(scopeClaim)) {
      return scopeClaim.map(value => String(value));
    }
    if (typeof scopeClaim === 'string') {
      return [scopeClaim];
    }
    return [];
  }

  isTokenExpired(token?: string): boolean {
    const payload = this.decodeToken(token);
    const exp = payload?.['exp'];
    if (typeof exp !== 'number') {
      return true;
    }

    return exp * 1000 <= Date.now();
  }
}
