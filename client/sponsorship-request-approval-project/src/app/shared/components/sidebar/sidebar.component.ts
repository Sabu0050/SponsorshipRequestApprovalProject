import { Component, EventEmitter, Input, Output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { ApplicationRole } from '../../../core/models/roles.model';
import { AuthService } from '../../../core/services/auth.service';

interface NavItem {
  label: string;
  path: string;
  roles?: ApplicationRole[];
  authorities?: Array<'RequestorAccess' | 'ManagerApproval' | 'FinanceApproval'>;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <aside class="sidebar" [class.open]="open" [class.mobile]="mobile">
      @for (item of items; track item.path) {
        @if (canShow(item)) {
          <a [routerLink]="item.path" routerLinkActive="active" (click)="onNavigate()">{{ item.label }}</a>
        }
      }
    </aside>
  `,
  styles: [`
    .sidebar { background: linear-gradient(180deg, #0f172a 0%, #1e293b 100%); color: #d1fae5; display: flex; flex-direction: column; padding: 12px; gap: 6px; border-right: 1px solid #334155; width: 220px; }
    a { color: #d1fae5; text-decoration: none; padding: 9px 10px; border-radius: 10px; border: 1px solid transparent; font-weight: 600; }
    a.active { background: rgba(20, 184, 166, 0.22); border-color: rgba(45, 212, 191, 0.42); color: #f0fdfa; }
    a:hover { background: rgba(20, 184, 166, 0.15); color: #f0fdfa; }
    @media (max-width: 992px) {
      .sidebar.mobile {
        position: fixed;
        top: 56px;
        left: -240px;
        height: calc(100vh - 56px);
        width: 220px;
        transition: left .2s ease;
        z-index: 45;
        overflow-y: auto;
        pointer-events: none;
      }
      .sidebar.mobile.open {
        left: 0;
        pointer-events: auto;
      }
    }
  `]
})
export class SidebarComponent {
  @Input() open = true;
  @Input() mobile = false;
  @Output() readonly navigate = new EventEmitter<void>();

  readonly items: NavItem[] = [
    { label: 'My Requests', path: '/my-requests', roles: [ApplicationRole.Requestor], authorities: ['RequestorAccess'] },
    { label: 'Manager Approvals', path: '/manager-approvals', roles: [ApplicationRole.Manager], authorities: ['ManagerApproval'] },
    { label: 'Finance Approvals', path: '/finance-approvals', roles: [ApplicationRole.FinanceAdmin], authorities: ['FinanceApproval'] },
    { label: 'Admin Workspace', path: '/admin/requests', roles: [ApplicationRole.SystemAdmin] }
  ];

  constructor(public readonly authService: AuthService) {}

  canShow(item: NavItem): boolean {
    if (item.authorities?.length) {
      return item.authorities.some(authority => this.authService.hasAuthority(authority));
    }

    return item.roles?.length ? this.authService.hasAnyRole(item.roles) : false;
  }

  onNavigate(): void {
    this.navigate.emit();
  }
}
