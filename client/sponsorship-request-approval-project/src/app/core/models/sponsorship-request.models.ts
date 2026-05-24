export type SponsorshipRequestStatus =
  | 'Draft'
  | 'PendingManagerApproval'
  | 'PendingFinanceReview'
  | 'Approved'
  | 'Rejected'
  | 'Cancelled';

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface SponsorshipRequestListItem {
  id: string;
  requestNumber: string;
  title: string;
  sponsorshipTypeName: string;
  requesterName: string;
  sponsorName: string;
  requestedAmount: number;
  currencyCode: string;
  status: SponsorshipRequestStatus;
  currentApproverId?: string;
  currentApproverName?: string;
  createdAt: string;
  submittedAt?: string;
  approvedAt?: string;
  rejectedAt?: string;
}

export interface RequestAttachment {
  id: string;
  fileName: string;
  contentType: string;
  fileSizeBytes: number;
  uploadedByName: string;
  uploadedAt: string;
}

export interface SponsorshipRequestDetail extends SponsorshipRequestListItem {
  description: string;
  sponsorshipTypeId: string;
  requesterId: string;
  requesterEmail: string;
  eventDate?: string;
  sponsorshipStartDate?: string;
  sponsorshipEndDate?: string;
  updatedAt?: string;
  finalDecisionById?: string;
  finalDecisionByName?: string;
  decisionNotes?: string;
  attachments: RequestAttachment[];
}

export interface WorkflowHistory {
  id: string;
  sponsorshipRequestId: string;
  action: string;
  fromStatus?: SponsorshipRequestStatus;
  toStatus: SponsorshipRequestStatus;
  performedById: string;
  performedByName: string;
  assignedToId?: string;
  assignedToName?: string;
  remarks?: string;
  performedAt: string;
}

export interface DashboardSummary {
  total: number;
  draft: number;
  pendingManagerApproval: number;
  pendingFinanceReview: number;
  approved: number;
  rejected: number;
  cancelled: number;
}

export interface SaveSponsorshipRequestPayload {
  title: string;
  description: string;
  sponsorshipTypeId: string;
  sponsorName: string;
  requestedAmount: number;
  currencyCode: string;
  eventDate?: string | null;
  sponsorshipStartDate?: string | null;
  sponsorshipEndDate?: string | null;
}

export interface SaveSponsorshipRequestResult {
  id: string;
  requestNumber: string;
  status: SponsorshipRequestStatus;
}
