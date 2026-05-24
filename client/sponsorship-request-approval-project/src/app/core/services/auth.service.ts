import { Injectable, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, tap } from 'rxjs';
import { LoginOption, LoginRequest, LoginResponse } from '../models/auth.models';
import { ApplicationRole } from '../models/roles.model';
import { CurrentUser } from '../models/user.models';
import { ApiService } from './api.service';
import { TokenService } from './token.service';

const USER_KEY = 'sponsorship.user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly userState = signal<CurrentUser | null>(this.readUser());
  readonly currentUser = this.userState.asReadonly();

  constructor(
    private readonly apiService: ApiService,
    private readonly tokenService: TokenService,
    private readonly router: Router
  ) {}

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.apiService.post<LoginResponse, LoginRequest>('auth/login', request).pipe(
      tap(response => this.setAuth(response))
    );
  }

  getLoginOptions(): Observable<LoginOption[]> {
    return this.apiService.get<LoginOption[]>('auth/login-options');
  }

  getCurrentUser(): Observable<CurrentUser | null> {
    return of(this.userState());
  }

  logout(): void {
    this.tokenService.removeToken();
    localStorage.removeItem(USER_KEY);
    this.userState.set(null);
    this.router.navigate(['/login']);
  }

  isAuthenticated(): boolean {
    const token = this.tokenService.getToken();
    return !!token && !this.tokenService.isTokenExpired(token);
  }

  hasRole(role: string): boolean {
    return this.getUserRoles().includes(role);
  }

  hasAnyRole(roles: string[]): boolean {
    return roles.some(role => this.hasRole(role));
  }

  hasApprovalAuthority(stage: 'ManagerApproval' | 'FinanceApproval'): boolean {
    return this.tokenService.getApprovalStagesFromToken()
      .map(value => value.trim().toLowerCase())
      .includes(stage.toLowerCase());
  }

  hasRequestorAccess(): boolean {
    const hasRoleBasedRequestor = this.hasRole(ApplicationRole.Requestor);
    const accessScopes = this.tokenService.getAccessScopesFromToken()
      .map(value => value.trim().toLowerCase());

    return hasRoleBasedRequestor
      || accessScopes.includes('requestoraccess')
      || accessScopes.includes('requestor');
  }

  hasAuthority(authority: 'RequestorAccess' | 'ManagerApproval' | 'FinanceApproval'): boolean {
    if (authority === 'RequestorAccess') {
      return this.hasRequestorAccess();
    }
    return this.hasApprovalAuthority(authority);
  }

  getLandingRoute(): string {
    if (this.hasRole(ApplicationRole.SystemAdmin)) {
      return '/admin/requests';
    }
    if (this.hasApprovalAuthority('FinanceApproval')) {
      return '/finance-approvals';
    }
    if (this.hasApprovalAuthority('ManagerApproval')) {
      return '/manager-approvals';
    }
    if (this.hasRequestorAccess()) {
      return '/my-requests';
    }
    return '/login';
  }

  getUserRoles(): string[] {
    const localRoles = this.userState()?.roles;
    if (localRoles?.length) {
      return localRoles;
    }
    return this.tokenService.getRolesFromToken();
  }

  getDisplayName(): string {
    const user = this.userState();
    const fullName = user?.fullName?.trim();
    if (fullName && fullName !== user?.email) {
      return fullName;
    }

    const tokenPayload = this.tokenService.decodeToken();
    const tokenName = this.readNameFromToken(tokenPayload);
    if (tokenName) {
      return tokenName;
    }

    return user?.email ?? '';
  }

  private setAuth(response: LoginResponse): void {
    const responseWithOptionalName = response as LoginResponse & {
      fullName?: string;
      firstName?: string;
      lastName?: string;
      name?: string;
    };

    const composedName = `${responseWithOptionalName.firstName ?? ''} ${responseWithOptionalName.lastName ?? ''}`.trim();
    const fullNameFromResponse = (
      responseWithOptionalName.fullName?.trim()
      || responseWithOptionalName.name?.trim()
      || composedName
    );
    const fullNameFromToken = this.readNameFromToken(this.tokenService.decodeToken(response.accessToken));
    const resolvedName = fullNameFromResponse || fullNameFromToken || response.email;

    this.tokenService.setToken(response.accessToken);
    this.storeUser({
      userId: response.userId,
      email: response.email,
      fullName: resolvedName,
      roles: response.roles
    });
  }

  private readNameFromToken(payload: Record<string, unknown> | null): string {
    if (!payload) {
      return '';
    }

    const claimName = String(
      payload['name']
      ?? payload['unique_name']
      ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
      ?? ''
    ).trim();

    if (claimName) {
      return claimName;
    }

    const givenName = String(
      payload['given_name']
      ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname']
      ?? ''
    ).trim();
    const familyName = String(
      payload['family_name']
      ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname']
      ?? ''
    ).trim();

    return `${givenName} ${familyName}`.trim();
  }

  private storeUser(user: CurrentUser): void {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
    this.userState.set(user);
  }

  private readUser(): CurrentUser | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as CurrentUser;
    } catch {
      localStorage.removeItem(USER_KEY);
      return null;
    }
  }
}
