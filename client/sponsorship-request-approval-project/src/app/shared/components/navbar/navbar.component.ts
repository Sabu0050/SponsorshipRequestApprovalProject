import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-navbar',
  standalone: true,
  template: `
    <header class="navbar">
      <div class="brand">
        <span class="dot"></span>
        SponsorDesk
      </div>
      <div class="meta">
        <span class="user">{{ authService.getDisplayName() }}</span>
        <span class="role">{{ authService.getUserRoles().length ? authService.getUserRoles()[0] : 'User' }}</span>
        <button type="button" (click)="authService.logout()">Logout</button>
      </div>
    </header>
  `,
  styles: [`
    .navbar { height: 56px; background: linear-gradient(90deg, #1f2937 0%, #0f766e 100%); color: #ecfeff; display: flex; align-items: center; justify-content: space-between; padding: 0 16px; border-bottom: 1px solid #134e4a; }
    .brand { font-weight: 650; display: flex; align-items: center; gap: 8px; letter-spacing: 0; }
    .dot { width: 9px; height: 9px; border-radius: 50%; background: #34d399; box-shadow: 0 0 0 3px rgba(52,211,153,.22); }
    .meta { display: flex; gap: 10px; align-items: center; }
    .user { color: #d1fae5; }
    .role { background: rgba(17, 24, 39, 0.28); border: 1px solid rgba(209, 250, 229, 0.35); padding: 2px 8px; border-radius: 999px; font-size: 12px; color: #ecfeff; }
    button { background: #991b1b; border: 1px solid #ef4444; color: #fff; padding: 6px 10px; border-radius: 8px; cursor: pointer; }
  `]
})
export class NavbarComponent {
  constructor(public readonly authService: AuthService) {}
}
