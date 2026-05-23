import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CancelSponsorshipRequestPayload,
  FinanceApprovalPayload,
  ManagerApprovalPayload,
  RejectSponsorshipRequestPayload,
  SponsorshipRequestWorkflowResult,
  SubmitSponsorshipRequestPayload
} from '../models/sponsorship-request.models';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class WorkflowActionsService {
  constructor(private readonly apiService: ApiService) {
  }

  submitRequest(
    sponsorshipRequestId: string,
    payload: SubmitSponsorshipRequestPayload
  ): Observable<SponsorshipRequestWorkflowResult> {
    return this.apiService.post<SponsorshipRequestWorkflowResult, SubmitSponsorshipRequestPayload>(
      `sponsorship-requests/${sponsorshipRequestId}/submissions`,
      payload
    );
  }

  approveByManager(
    sponsorshipRequestId: string,
    payload: ManagerApprovalPayload
  ): Observable<SponsorshipRequestWorkflowResult> {
    return this.apiService.post<SponsorshipRequestWorkflowResult, ManagerApprovalPayload>(
      `sponsorship-requests/${sponsorshipRequestId}/manager-approvals`,
      payload
    );
  }

  approveByFinance(
    sponsorshipRequestId: string,
    payload: FinanceApprovalPayload
  ): Observable<SponsorshipRequestWorkflowResult> {
    return this.apiService.post<SponsorshipRequestWorkflowResult, FinanceApprovalPayload>(
      `sponsorship-requests/${sponsorshipRequestId}/finance-approvals`,
      payload
    );
  }

  rejectRequest(
    sponsorshipRequestId: string,
    payload: RejectSponsorshipRequestPayload
  ): Observable<SponsorshipRequestWorkflowResult> {
    return this.apiService.post<SponsorshipRequestWorkflowResult, RejectSponsorshipRequestPayload>(
      `sponsorship-requests/${sponsorshipRequestId}/rejections`,
      payload
    );
  }

  cancelRequest(
    sponsorshipRequestId: string,
    payload: CancelSponsorshipRequestPayload
  ): Observable<SponsorshipRequestWorkflowResult> {
    return this.apiService.post<SponsorshipRequestWorkflowResult, CancelSponsorshipRequestPayload>(
      `sponsorship-requests/${sponsorshipRequestId}/cancellations`,
      payload
    );
  }
}
