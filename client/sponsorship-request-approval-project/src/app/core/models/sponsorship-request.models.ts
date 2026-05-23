export type SponsorshipRequestStatus =
  | 'Draft'
  | 'PendingManagerApproval'
  | 'PendingFinanceReview'
  | 'Approved'
  | 'Rejected'
  | 'Cancelled';

export type WorkflowAction =
  | 'Created'
  | 'Submitted'
  | 'ManagerApproved'
  | 'FinanceApproved'
  | 'Rejected'
  | 'Cancelled';

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
  currentApproverId?: string | null;
  currentApproverName?: string | null;
  createdAt: string;
  submittedAt?: string | null;
  approvedAt?: string | null;
  rejectedAt?: string | null;
}

export interface SponsorshipRequestDetail extends SponsorshipRequestListItem {
  description: string;
  sponsorshipTypeId: string;
  requesterId: string;
  requesterEmail: string;
  eventDate?: string | null;
  sponsorshipStartDate?: string | null;
  sponsorshipEndDate?: string | null;
  updatedAt?: string | null;
  finalDecisionById?: string | null;
  finalDecisionByName?: string | null;
  decisionNotes?: string | null;
  attachments: RequestAttachment[];
}

export interface RequestAttachment {
  id: string;
  fileName: string;
  contentType: string;
  fileSizeBytes: number;
  uploadedByName: string;
  uploadedAt: string;
}

export interface WorkflowHistory {
  id: string;
  sponsorshipRequestId: string;
  action: WorkflowAction;
  fromStatus?: SponsorshipRequestStatus | null;
  toStatus: SponsorshipRequestStatus;
  performedById: string;
  performedByName: string;
  assignedToId?: string | null;
  assignedToName?: string | null;
  comments?: string | null;
  performedAt: string;
}
