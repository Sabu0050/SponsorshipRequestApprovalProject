import { Injectable } from '@angular/core';
import { Observable, catchError, map, of } from 'rxjs';
import { DashboardSummary, SponsorshipRequestListItem } from '../models/sponsorship-request.models';
import { ApiService } from './api.service';
import { SponsorshipRequestService } from './sponsorship-request.service';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  constructor(
    private readonly apiService: ApiService,
    private readonly requestService: SponsorshipRequestService
  ) {}

  getSummary(): Observable<DashboardSummary> {
    return this.apiService.get<DashboardSummary>('dashboard/summary').pipe(
      catchError(() => this.getFallbackSummary())
    );
  }

  private getFallbackSummary(): Observable<DashboardSummary> {
    return this.requestService.getAll(1, 100).pipe(map(result => this.computeSummary(result.items)));
  }

  private computeSummary(list: SponsorshipRequestListItem[]): DashboardSummary {
    return {
      total: list.length,
      draft: list.filter(item => item.status === 'Draft').length,
      pendingManagerApproval: list.filter(item => item.status === 'PendingManagerApproval').length,
      pendingFinanceReview: list.filter(item => item.status === 'PendingFinanceReview').length,
      approved: list.filter(item => item.status === 'Approved').length,
      rejected: list.filter(item => item.status === 'Rejected').length,
      cancelled: list.filter(item => item.status === 'Cancelled').length
    };
  }
}
