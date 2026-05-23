import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { LoginRequest, LoginResult } from '../models/auth.models';
import { ApiService } from './api.service';

const tokenStorageKey = 'sponsorship.accessToken';
const authStorageKey = 'sponsorship.auth';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly authState = signal<LoginResult | null>(this.readStoredAuth());

  readonly currentUser = this.authState.asReadonly();

  constructor(
    private readonly apiService: ApiService,
    private readonly router: Router
  ) {
  }

  login(request: LoginRequest) {
    return this.apiService.post<LoginResult, LoginRequest>('auth/login', request)
      .pipe(tap(result => this.storeAuth(result)));
  }

  logout(): void {
    localStorage.removeItem(tokenStorageKey);
    localStorage.removeItem(authStorageKey);
    this.authState.set(null);
    this.router.navigate(['/auth/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(tokenStorageKey);
  }

  isAuthenticated(): boolean {
    const auth = this.authState();
    return !!auth?.accessToken && new Date(auth.expiresAt).getTime() > Date.now();
  }

  hasAnyRole(roles: readonly string[]): boolean {
    const userRoles = this.authState()?.roles ?? [];
    return roles.some(role => userRoles.includes(role));
  }

  private storeAuth(result: LoginResult): void {
    localStorage.setItem(tokenStorageKey, result.accessToken);
    localStorage.setItem(authStorageKey, JSON.stringify(result));
    this.authState.set(result);
  }

  private readStoredAuth(): LoginResult | null {
    const raw = localStorage.getItem(authStorageKey);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as LoginResult;
    } catch {
      localStorage.removeItem(authStorageKey);
      localStorage.removeItem(tokenStorageKey);
      return null;
    }
  }
}
