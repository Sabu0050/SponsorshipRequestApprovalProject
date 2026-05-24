import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface AdminRole {
  name: string;
  canRequestorAccess: boolean;
  canApproveManagerStage: boolean;
  canApproveFinanceStage: boolean;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(private readonly apiService: ApiService) {}

  private toBool(value: unknown): boolean {
    if (typeof value === 'boolean') return value;
    if (typeof value === 'number') return value === 1;
    if (typeof value === 'string') {
      const normalized = value.trim().toLowerCase();
      return normalized === 'true' || normalized === '1' || normalized === 'yes';
    }
    return false;
  }

  getRoles(): Observable<AdminRole[]> {
    return this.apiService.get<unknown>('admin/roles').pipe(
      map(payload => {
        const items = Array.isArray(payload)
          ? payload
          : Array.isArray((payload as { data?: unknown[] })?.data)
            ? ((payload as { data?: unknown[] }).data ?? [])
            : [];

        return items.map(item => {
          const row = item as Record<string, unknown>;
          return {
            name: String(row['name'] ?? row['Name'] ?? ''),
            canRequestorAccess: this.toBool(row['canRequestorAccess'] ?? row['CanRequestorAccess']),
            canApproveManagerStage: this.toBool(row['canApproveManagerStage'] ?? row['CanApproveManagerStage']),
            canApproveFinanceStage: this.toBool(row['canApproveFinanceStage'] ?? row['CanApproveFinanceStage'])
          };
        }).filter(item => item.name);
      })
    );
  }

  createRole(payload: {
    name: string;
    canRequestorAccess: boolean;
    canApproveManagerStage: boolean;
    canApproveFinanceStage: boolean;
  }): Observable<string> {
    return this.apiService.post<unknown, typeof payload>('admin/roles', payload).pipe(
      map(response => {
        const row = response as Record<string, unknown>;
        return String(row['name'] ?? row['Name'] ?? payload.name);
      }));
  }

  updateRoleAuthorities(
    roleName: string,
    payload: {
      canRequestorAccess: boolean;
      canApproveManagerStage: boolean;
      canApproveFinanceStage: boolean;
    }): Observable<unknown> {
    return this.apiService.put<unknown, typeof payload>(`admin/roles/${encodeURIComponent(roleName)}`, payload);
  }

  getUsers(): Observable<Array<{
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    department?: string;
    role: string;
    canRequestorAccess: boolean;
    canApproveManagerStage: boolean;
    canApproveFinanceStage: boolean;
  }>> {
    return this.apiService.get<unknown>('admin/users').pipe(
      map(payload => {
        const items = Array.isArray(payload)
          ? payload
          : Array.isArray((payload as { data?: unknown[] })?.data)
            ? ((payload as { data?: unknown[] }).data ?? [])
            : [];

        return items.map(item => {
          const row = item as Record<string, unknown>;
          return {
            id: String(row['id'] ?? ''),
            email: String(row['email'] ?? ''),
            firstName: String(row['firstName'] ?? ''),
            lastName: String(row['lastName'] ?? ''),
            department: String(row['department'] ?? ''),
            role: String(row['role'] ?? ''),
            canRequestorAccess: this.toBool(row['canRequestorAccess'] ?? row['CanRequestorAccess']),
            canApproveManagerStage: this.toBool(row['canApproveManagerStage'] ?? row['CanApproveManagerStage']),
            canApproveFinanceStage: this.toBool(row['canApproveFinanceStage'] ?? row['CanApproveFinanceStage'])
          };
        });
      })
    );
  }

  createUser(payload: {
    email: string;
    password: string;
    firstName: string;
    lastName: string;
    department?: string;
    role: string;
  }): Observable<unknown> {
    return this.apiService.post<unknown, typeof payload>('admin/users', payload);
  }

  updateUserAuthorities(
    userId: string,
    payload: {
      firstName: string;
      lastName: string;
      department?: string;
      role: string;
    }): Observable<unknown> {
    return this.apiService.put<unknown, typeof payload>(`admin/users/${userId}`, payload);
  }
}
