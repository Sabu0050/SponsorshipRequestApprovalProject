import { Component, Input } from '@angular/core';
import { TagModule } from 'primeng/tag';
import { normalizeStatus } from '../../utils/status.utils';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [TagModule],
  template: `<p-tag [value]="normalizeStatus(status)" [severity]="severity()" />`,
  styles: [`
  `]
})
export class StatusBadgeComponent {
  @Input({ required: true }) status = '';
  protected readonly normalizeStatus = normalizeStatus;

  severity(): 'secondary' | 'info' | 'success' | 'warn' | 'danger' | 'contrast' {
    const status = normalizeStatus(this.status);
    if (status === 'Approved') return 'success';
    if (status === 'Rejected' || status === 'Cancelled') return 'danger';
    if (status === 'PendingManagerApproval' || status === 'PendingFinanceReview') return 'warn';
    return 'secondary';
  }
}
