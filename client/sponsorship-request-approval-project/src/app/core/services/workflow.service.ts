import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult, WorkflowHistory } from '../models/sponsorship-request.models';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class WorkflowService {
  constructor(private readonly apiService: ApiService) {}

  getHistory(sponsorshipRequestId: string, pageNumber = 1, pageSize = 20): Observable<PagedResult<WorkflowHistory>> {
    return this.apiService.get<PagedResult<WorkflowHistory>>(`sponsorship-requests/${sponsorshipRequestId}/workflow-history`, { pageNumber, pageSize });
  }

  submit(sponsorshipRequestId: string, payload: { assignedManagerId?: string; assignedManagerName?: string; comments?: string }): Observable<unknown> {
    return this.apiService.post<unknown, unknown>(`sponsorship-requests/${sponsorshipRequestId}/submissions`, payload);
  }

  managerApprove(sponsorshipRequestId: string, payload: { assignedFinanceReviewerId?: string; assignedFinanceReviewerName?: string; comments?: string }): Observable<unknown> {
    return this.apiService.post<unknown, unknown>(`sponsorship-requests/${sponsorshipRequestId}/manager-approvals`, payload);
  }

  financeApprove(sponsorshipRequestId: string, payload: { comments?: string }): Observable<unknown> {
    return this.apiService.post<unknown, unknown>(`sponsorship-requests/${sponsorshipRequestId}/finance-approvals`, payload);
  }

  reject(sponsorshipRequestId: string, payload: { expectedCurrentStatus: string | number; comments: string }): Observable<unknown> {
    return this.apiService.post<unknown, unknown>(`sponsorship-requests/${sponsorshipRequestId}/rejections`, payload);
  }

  cancel(sponsorshipRequestId: string, payload: { expectedCurrentStatus: string | number; comments?: string }): Observable<unknown> {
    return this.apiService.post<unknown, unknown>(`sponsorship-requests/${sponsorshipRequestId}/cancellations`, payload);
  }
}
