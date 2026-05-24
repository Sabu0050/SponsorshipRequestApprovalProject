import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SponsorshipRequestListItem } from '../../../core/models/sponsorship-request.models';
import { SponsorshipRequestService } from '../../../core/services/sponsorship-request.service';
import { WorkflowService } from '../../../core/services/workflow.service';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { normalizeStatus, toStatusCode } from '../../../shared/utils/status.utils';
import { RequestCreateComponent } from '../request-create/request-create.component';

@Component({
  selector: 'app-my-requests',
  standalone: true,
  imports: [RouterLink, StatusBadgeComponent, RequestCreateComponent],
  templateUrl: './my-requests.component.html',
  styleUrl: './my-requests.component.css'
})
export class MyRequestsComponent implements OnInit {
  items: SponsorshipRequestListItem[] = [];
  showCreate = true;
  constructor(private readonly service: SponsorshipRequestService, private readonly workflowService: WorkflowService) {}
  ngOnInit(): void { this.load(); }
  load(): void { this.service.getMyRequests(1, 50).subscribe(result => this.items = result.items); }
  submit(item: SponsorshipRequestListItem): void { this.workflowService.submit(item.id, { comments: '' }).subscribe(() => this.load()); }
  cancel(item: SponsorshipRequestListItem): void {
    this.workflowService.cancel(item.id, { expectedCurrentStatus: toStatusCode(item.status), comments: '' }).subscribe(() => this.load());
  }
  isStatus(item: SponsorshipRequestListItem, status: string): boolean {
    return normalizeStatus(item.status) === status;
  }

  get draftCount(): number {
    return this.items.filter(item => normalizeStatus(item.status) === 'Draft').length;
  }

  get pendingManagerCount(): number {
    return this.items.filter(item => normalizeStatus(item.status) === 'PendingManagerApproval').length;
  }

  get pendingFinanceCount(): number {
    return this.items.filter(item => normalizeStatus(item.status) === 'PendingFinanceReview').length;
  }

  get approvedCount(): number {
    return this.items.filter(item => normalizeStatus(item.status) === 'Approved').length;
  }

  get rejectedCount(): number {
    return this.items.filter(item => normalizeStatus(item.status) === 'Rejected').length;
  }

  get cancelledCount(): number {
    return this.items.filter(item => normalizeStatus(item.status) === 'Cancelled').length;
  }
}
