import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { ApplicationRole } from './core/models/roles.model';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'sponsorship-requests'
  },
  {
    path: 'auth/login',
    loadComponent: () => import('./features/auth/auth-page.component')
      .then(component => component.AuthPageComponent)
  },
  {
    path: 'sponsorship-requests',
    canActivate: [authGuard],
    loadChildren: () => import('./features/requests/requests.module')
      .then(module => module.RequestsModule)
  },
  {
    path: 'approvals',
    canActivate: [authGuard, roleGuard],
    data: {
      roles: [ApplicationRole.Manager, ApplicationRole.FinanceAdmin]
    },
    loadChildren: () => import('./features/approvals/approvals.module')
      .then(module => module.ApprovalsModule)
  },
  {
    path: 'admin',
    canActivate: [authGuard, roleGuard],
    data: {
      roles: [ApplicationRole.SystemAdmin]
    },
    loadChildren: () => import('./features/admin/admin.module')
      .then(module => module.AdminModule)
  },
  {
    path: '**',
    redirectTo: 'sponsorship-requests'
  }
];
