export interface DashboardSummary {
  total: number;
  draft: number;
  pendingManagerApproval: number;
  pendingFinanceReview: number;
  approved: number;
  rejected: number;
  cancelled: number;
}
