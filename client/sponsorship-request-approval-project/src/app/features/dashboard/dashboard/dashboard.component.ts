import { Component, OnInit } from '@angular/core';
import { DashboardSummary } from '../../../core/models/sponsorship-request.models';
import { DashboardService } from '../../../core/services/dashboard.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <section class="page">
      <h2>Dashboard</h2>
      <div class="cards">
        <article><h4>Total</h4><p>{{ summary.total }}</p></article>
        <article><h4>Draft</h4><p>{{ summary.draft }}</p></article>
        <article><h4>Pending Manager</h4><p>{{ summary.pendingManagerApproval }}</p></article>
        <article><h4>Pending Finance</h4><p>{{ summary.pendingFinanceReview }}</p></article>
        <article><h4>Approved</h4><p>{{ summary.approved }}</p></article>
        <article><h4>Rejected</h4><p>{{ summary.rejected }}</p></article>
        <article><h4>Cancelled</h4><p>{{ summary.cancelled }}</p></article>
      </div>
    </section>
  `,
  styles: [`p{margin:8px 0 0}`]
})
export class DashboardComponent implements OnInit {
  summary: DashboardSummary = { total: 0, draft: 0, pendingManagerApproval: 0, pendingFinanceReview: 0, approved: 0, rejected: 0, cancelled: 0 };

  constructor(private readonly dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.dashboardService.getSummary().subscribe(result => this.summary = result);
  }
}
