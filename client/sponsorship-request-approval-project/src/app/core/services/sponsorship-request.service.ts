import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  PagedResult,
  SaveSponsorshipRequestPayload,
  SaveSponsorshipRequestResult,
  SponsorshipRequestDetail,
  SponsorshipRequestListItem,
  SponsorshipRequestStatus
} from '../models/sponsorship-request.models';
import { ApiService } from './api.service';

@Injectable({ providedIn: 'root' })
export class SponsorshipRequestService {
  constructor(private readonly apiService: ApiService) {}

  getAll(pageNumber = 1, pageSize = 20, status?: SponsorshipRequestStatus): Observable<PagedResult<SponsorshipRequestListItem>> {
    return this.apiService.get<PagedResult<SponsorshipRequestListItem>>('sponsorship-requests', { pageNumber, pageSize, status });
  }

  getById(id: string): Observable<SponsorshipRequestDetail> {
    return this.apiService.get<SponsorshipRequestDetail>(`sponsorship-requests/${id}`);
  }

  getMyRequests(pageNumber = 1, pageSize = 20, status?: SponsorshipRequestStatus): Observable<PagedResult<SponsorshipRequestListItem>> {
    return this.getAll(pageNumber, pageSize, status);
  }

  getPendingManagerApprovals(pageNumber = 1, pageSize = 20): Observable<PagedResult<SponsorshipRequestListItem>> {
    return this.getAll(pageNumber, pageSize, 'PendingManagerApproval');
  }

  getPendingFinanceReviews(pageNumber = 1, pageSize = 20): Observable<PagedResult<SponsorshipRequestListItem>> {
    return this.getAll(pageNumber, pageSize, 'PendingFinanceReview');
  }

  create(payload: SaveSponsorshipRequestPayload): Observable<SaveSponsorshipRequestResult> {
    return this.apiService.post<SaveSponsorshipRequestResult, SaveSponsorshipRequestPayload>(
      'sponsorship-requests',
      payload
    );
  }

  update(id: string, payload: SaveSponsorshipRequestPayload): Observable<SaveSponsorshipRequestResult> {
    return this.apiService.put<SaveSponsorshipRequestResult, SaveSponsorshipRequestPayload>(
      `sponsorship-requests/${id}`,
      payload
    );
  }
}
