import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';

export interface Role {
  name: string;
}

@Injectable({ providedIn: 'root' })
export class AdminService {
  constructor(private readonly apiService: ApiService) {
  }

  getRoles(): Observable<Role[]> {
    return this.apiService.get<Role[]>('admin/roles');
  }
}
