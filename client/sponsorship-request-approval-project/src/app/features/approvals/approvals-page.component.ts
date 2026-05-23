import { Component, OnInit } from '@angular/core';
import { PagedResult } from '../../core/models/api.models';
import { SponsorshipRequestListItem } from '../../core/models/sponsorship-request.models';
import { SponsorshipRequestsService } from '../../core/services/sponsorship-requests.service';

@Component({
  selector: 'app-approvals-page',
  standalone: false,
  templateUrl: './approvals-page.component.html'
})
export class ApprovalsPageComponent implements OnInit {
  protected approvals?: PagedResult<SponsorshipRequestListItem>;

  constructor(private readonly requestsService: SponsorshipRequestsService) {
  }

  ngOnInit(): void {
    this.requestsService.getRequests({ pageNumber: 1, pageSize: 20 })
      .subscribe(result => this.approvals = result);
  }
}
