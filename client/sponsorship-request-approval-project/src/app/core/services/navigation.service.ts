import { computed, Injectable } from '@angular/core';
import { ApplicationRole } from '../models/roles.model';
import { AuthService } from './auth.service';

export interface NavigationItem {
  label: string;
  path: string;
  roles?: ApplicationRole[];
}

const navigationItems: NavigationItem[] = [
  { label: 'Requests', path: '/sponsorship-requests' },
  { label: 'Approvals', path: '/approvals', roles: [ApplicationRole.Manager, ApplicationRole.FinanceAdmin] },
  { label: 'Admin', path: '/admin', roles: [ApplicationRole.SystemAdmin] }
];

@Injectable({ providedIn: 'root' })
export class NavigationService {
  readonly visibleItems = computed(() => navigationItems.filter(item =>
    !item.roles?.length || this.authService.hasAnyRole(item.roles)));

  constructor(private readonly authService: AuthService) {
  }
}
