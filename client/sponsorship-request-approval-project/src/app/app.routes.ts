import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard } from './core/guards/role.guard';
import { ApplicationRole } from './core/models/roles.model';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'login' },
  { path: 'login', loadComponent: () => import('./features/auth/login/login.component').then(c => c.LoginComponent) },
  { path: 'dashboard', pathMatch: 'full', redirectTo: 'admin/requests' },
  { path: 'sponsorship-requests/create', pathMatch: 'full', redirectTo: 'my-requests' },
  { path: 'sponsorship-requests/:id/edit', canActivate: [authGuard, roleGuard], data: { roles: [ApplicationRole.Requestor] }, loadComponent: () => import('./features/sponsorship-requests/request-edit/request-edit.component').then(c => c.RequestEditComponent) },
  { path: 'my-requests', canActivate: [authGuard, roleGuard], data: { roles: [ApplicationRole.Requestor], authorities: ['RequestorAccess'] }, loadComponent: () => import('./features/sponsorship-requests/my-requests/my-requests.component').then(c => c.MyRequestsComponent) },
  { path: 'sponsorship-requests/:id', canActivate: [authGuard], loadComponent: () => import('./features/sponsorship-requests/request-detail/request-detail.component').then(c => c.RequestDetailComponent) },
  { path: 'manager-approvals', canActivate: [authGuard, roleGuard], data: { roles: [ApplicationRole.Manager], authorities: ['ManagerApproval'] }, loadComponent: () => import('./features/approvals/manager-approvals/manager-approvals.component').then(c => c.ManagerApprovalsComponent) },
  { path: 'finance-approvals', canActivate: [authGuard, roleGuard], data: { roles: [ApplicationRole.FinanceAdmin], authorities: ['FinanceApproval'] }, loadComponent: () => import('./features/approvals/finance-approvals/finance-approvals.component').then(c => c.FinanceApprovalsComponent) },
  { path: 'admin/requests', canActivate: [authGuard, roleGuard], data: { roles: [ApplicationRole.SystemAdmin] }, loadComponent: () => import('./features/admin/all-requests/all-requests.component').then(c => c.AllRequestsComponent) },
  { path: 'admin/sponsorship-types', pathMatch: 'full', redirectTo: 'admin/requests' },
  { path: '**', redirectTo: 'login' }
];
