import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult } from '../models/api.models';
import {
  SponsorshipRequestDetail,
  SponsorshipRequestListItem,
  SponsorshipRequestStatus,
  WorkflowHistory
} from '../models/sponsorship-request.models';
import { ApiService } from './api.service';

export interface RequestListFilter {
  pageNumber: number;
  pageSize: number;
  status?: SponsorshipRequestStatus | null;
}

@Injectable({ providedIn: 'root' })
export class SponsorshipRequestsService {
  constructor(private readonly apiService: ApiService) {
  }

  getRequests(filter: RequestListFilter): Observable<PagedResult<SponsorshipRequestListItem>> {
    return this.apiService.get<PagedResult<SponsorshipRequestListItem>>('sponsorship-requests', filter);
  }

  getRequest(id: string): Observable<SponsorshipRequestDetail> {
    return this.apiService.get<SponsorshipRequestDetail>(`sponsorship-requests/${id}`);
  }

  getWorkflowHistory(id: string, pageNumber = 1, pageSize = 20): Observable<PagedResult<WorkflowHistory>> {
    return this.apiService.get<PagedResult<WorkflowHistory>>(
      `sponsorship-requests/${id}/workflow-history`,
      { pageNumber, pageSize });
  }
}
