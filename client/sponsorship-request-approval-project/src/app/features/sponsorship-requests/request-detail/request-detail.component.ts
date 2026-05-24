import { Component, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SponsorshipRequestDetail, WorkflowHistory } from '../../../core/models/sponsorship-request.models';
import { AuthService } from '../../../core/services/auth.service';
import { SponsorshipRequestService } from '../../../core/services/sponsorship-request.service';
import { WorkflowService } from '../../../core/services/workflow.service';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { normalizeAction, normalizeStatus, toStatusCode } from '../../../shared/utils/status.utils';

@Component({
  selector: 'app-request-detail',
  standalone: true,
  imports: [FormsModule, StatusBadgeComponent, DatePipe],
  templateUrl: './request-detail.component.html',
  styleUrls: ['./request-detail.component.css']
})
export class RequestDetailComponent implements OnInit {
  item?: SponsorshipRequestDetail;
  history: WorkflowHistory[] = [];
  remarks = '';
  protected readonly normalizeStatus = normalizeStatus;
  protected readonly normalizeAction = normalizeAction;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly requestService: SponsorshipRequestService,
    private readonly workflowService: WorkflowService,
    public readonly authService: AuthService
  ) {}

  ngOnInit(): void { this.reload(); }

  reload(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { return; }
    this.requestService.getById(id).subscribe(item => this.item = item);
    this.workflowService.getHistory(id).subscribe(result => this.history = result.items);
  }

  act(action: 'Submit' | 'Cancel' | 'ManagerApprove' | 'ManagerReject' | 'FinanceApprove' | 'FinanceReject'): void {
    if (!this.item) { return; }
    const request = this.item;
    const call = action === 'Submit'
      ? this.workflowService.submit(request.id, { comments: this.remarks })
      : action === 'Cancel'
        ? this.workflowService.cancel(request.id, { expectedCurrentStatus: toStatusCode(request.status), comments: this.remarks })
        : action === 'ManagerApprove'
          ? this.workflowService.managerApprove(request.id, { comments: this.remarks })
          : action === 'ManagerReject'
            ? this.workflowService.reject(request.id, { expectedCurrentStatus: toStatusCode(request.status), comments: this.remarks || 'Rejected' })
            : action === 'FinanceApprove'
              ? this.workflowService.financeApprove(request.id, { comments: this.remarks })
              : this.workflowService.reject(request.id, { expectedCurrentStatus: toStatusCode(request.status), comments: this.remarks || 'Rejected' });
    call.subscribe(() => {
      this.remarks = '';
      this.reload();
    });
  }

  can(role: string): boolean { return this.authService.hasRole(role); }
}
