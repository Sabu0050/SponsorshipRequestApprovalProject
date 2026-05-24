import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { SponsorshipType } from '../models/sponsorship-type.models';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class SponsorshipTypeService {
  constructor(private readonly apiService: ApiService) {}

  private toBool(value: unknown, fallback = false): boolean {
    if (typeof value === 'boolean') return value;
    if (typeof value === 'number') return value === 1;
    if (typeof value === 'string') {
      const normalized = value.trim().toLowerCase();
      if (normalized === 'true' || normalized === '1' || normalized === 'yes') return true;
      if (normalized === 'false' || normalized === '0' || normalized === 'no') return false;
    }
    return fallback;
  }

  getAll(): Observable<SponsorshipType[]> {
    return this.apiService.get<unknown>('sponsorship-types').pipe(
      map(payload => {
        const items = Array.isArray(payload)
          ? payload
          : Array.isArray((payload as { data?: unknown[] })?.data)
            ? ((payload as { data?: unknown[] }).data ?? [])
            : [];

        return items.map(item => {
          const row = item as Record<string, unknown>;
          return {
            id: String(row['id'] ?? row['Id'] ?? ''),
            name: String(row['name'] ?? row['Name'] ?? ''),
            isActive: this.toBool(row['isActive'] ?? row['IsActive'], true)
          } satisfies SponsorshipType;
        }).filter(item => !!item.id && !!item.name);
      })
    );
  }

  create(payload: { name: string; description?: string; isActive?: boolean }): Observable<SponsorshipType> {
    return this.apiService.post<unknown, { name: string; description?: string; isActive?: boolean }>('sponsorship-types', payload).pipe(
      map(item => this.mapType(item))
    );
  }

  update(id: string, payload: { name: string; description?: string; isActive: boolean }): Observable<SponsorshipType> {
    return this.apiService.put<unknown, { name: string; description?: string; isActive: boolean }>(`sponsorship-types/${id}`, payload).pipe(
      map(item => this.mapType(item))
    );
  }

  private mapType(item: unknown): SponsorshipType {
    const row = item as Record<string, unknown>;
    return {
      id: String(row['id'] ?? row['Id'] ?? ''),
      name: String(row['name'] ?? row['Name'] ?? ''),
      isActive: this.toBool(row['isActive'] ?? row['IsActive'], true)
    } satisfies SponsorshipType;
  }
}
