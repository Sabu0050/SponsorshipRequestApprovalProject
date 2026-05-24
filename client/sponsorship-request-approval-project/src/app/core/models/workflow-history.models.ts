import { SponsorshipRequestStatus } from './sponsorship-request.models';

export interface WorkflowHistoryItem {
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
