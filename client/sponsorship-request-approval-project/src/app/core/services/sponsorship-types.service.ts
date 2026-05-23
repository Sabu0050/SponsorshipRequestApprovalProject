import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { SponsorshipType } from '../models/sponsorship-type.models';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class SponsorshipTypesService {
  constructor(private readonly apiService: ApiService) {
  }

  getTypes(isActive?: boolean): Observable<SponsorshipType[]> {
    return this.apiService.get<SponsorshipType[]>('sponsorship-types', { isActive });
  }

  getType(id: string): Observable<SponsorshipType> {
    return this.apiService.get<SponsorshipType>(`sponsorship-types/${id}`);
  }
}
