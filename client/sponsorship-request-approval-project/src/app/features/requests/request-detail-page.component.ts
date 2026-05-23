import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { SponsorshipRequestDetail } from '../../core/models/sponsorship-request.models';
import { SponsorshipRequestsService } from '../../core/services/sponsorship-requests.service';

@Component({
  selector: 'app-request-detail-page',
  standalone: false,
  templateUrl: './request-detail-page.component.html'
})
export class RequestDetailPageComponent implements OnInit {
  protected request?: SponsorshipRequestDetail;

  constructor(
    private readonly route: ActivatedRoute,
    private readonly requestsService: SponsorshipRequestsService
  ) {
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      return;
    }

    this.requestsService.getRequest(id).subscribe(result => this.request = result);
  }
}
