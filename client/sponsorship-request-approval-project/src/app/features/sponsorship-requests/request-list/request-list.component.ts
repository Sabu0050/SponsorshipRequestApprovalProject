import { Component, Input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { SponsorshipRequestListItem } from '../../../core/models/sponsorship-request.models';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';

@Component({
  selector: 'app-request-list',
  standalone: true,
  imports: [RouterLink, StatusBadgeComponent],
  template: `
    <div class="table-wrap">
      <table>
        <thead><tr><th>No</th><th>Title</th><th>Status</th><th></th></tr></thead>
        <tbody>
          @for (item of items; track item.id) {
            <tr><td>{{ item.requestNumber }}</td><td>{{ item.title }}</td><td><app-status-badge [status]="item.status" /></td><td><a [routerLink]="['/sponsorship-requests', item.id]">Detail</a></td></tr>
          }
        </tbody>
      </table>
    </div>
  `
})
export class RequestListComponent {
  @Input({ required: true }) items: SponsorshipRequestListItem[] = [];
}
