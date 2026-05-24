import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SponsorshipRequestListItem } from '../../../core/models/sponsorship-request.models';
import { SponsorshipRequestService } from '../../../core/services/sponsorship-request.service';
import { WorkflowService } from '../../../core/services/workflow.service';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { toStatusCode } from '../../../shared/utils/status.utils';

@Component({
  selector: 'app-manager-approvals',
  standalone: true,
  imports: [StatusBadgeComponent, FormsModule, DecimalPipe, DatePipe],
  templateUrl: './manager-approvals.component.html',
  styleUrl: './manager-approvals.component.css'
})
export class ManagerApprovalsComponent implements OnInit {
  @ViewChild('detailPanel') detailPanel?: ElementRef<HTMLElement>;
  @ViewChild('remarksBox') remarksBox?: ElementRef<HTMLTextAreaElement>;
  pending: SponsorshipRequestListItem[] = [];
  selectedRequest?: SponsorshipRequestListItem;
  remarks = '';
  busy = false;

  constructor(private readonly service: SponsorshipRequestService, private readonly workflowService: WorkflowService) {}
  ngOnInit(): void { this.load(); }

  load(): void {
    this.service.getPendingManagerApprovals(1, 50).subscribe(result => {
      this.pending = result.items;
      if (!this.selectedRequest && this.pending.length) {
        this.selectedRequest = this.pending[0];
      }
      if (this.selectedRequest && !this.pending.some(item => item.id === this.selectedRequest?.id)) {
        this.selectedRequest = this.pending[0];
      }
    });
  }

  select(item: SponsorshipRequestListItem): void {
    this.selectedRequest = item;
    this.remarks = '';
    setTimeout(() => {
      this.detailPanel?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
      this.remarksBox?.nativeElement.focus();
    }, 0);
  }

  approve(item: SponsorshipRequestListItem): void {
    if (this.busy) {
      return;
    }
    this.busy = true;
    this.workflowService.managerApprove(item.id, { comments: this.remarks }).subscribe({
      next: () => {
        this.remarks = '';
        this.busy = false;
        this.load();
      },
      error: () => {
        this.busy = false;
      }
    });
  }

  reject(item: SponsorshipRequestListItem): void {
    if (this.busy || !this.remarks.trim()) {
      return;
    }
    this.busy = true;
    this.workflowService.reject(item.id, { expectedCurrentStatus: toStatusCode(item.status), comments: this.remarks }).subscribe({
      next: () => {
        this.remarks = '';
        this.busy = false;
        this.load();
      },
      error: () => {
        this.busy = false;
      }
    });
  }

  get pendingAmount(): number {
    return this.pending.reduce((sum, item) => sum + (item.requestedAmount || 0), 0);
  }
}
