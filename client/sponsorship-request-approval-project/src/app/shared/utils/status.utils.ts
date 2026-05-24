const statusMap: Record<string, string> = {
  '0': 'Draft',
  '1': 'PendingManagerApproval',
  '2': 'PendingFinanceReview',
  '4': 'Approved',
  '5': 'Rejected',
  '7': 'Cancelled'
};

const actionMap: Record<string, string> = {
  '0': 'Created',
  '1': 'Submitted',
  '2': 'ManagerApproved',
  '3': 'FinanceApproved',
  '5': 'Rejected',
  '7': 'Cancelled'
};

export function normalizeStatus(value: unknown): string {
  const key = String(value ?? '');
  return statusMap[key] ?? key;
}

export function normalizeAction(value: unknown): string {
  const key = String(value ?? '');
  return actionMap[key] ?? key;
}

export function toStatusCode(value: unknown): number {
  const normalized = normalizeStatus(value);
  if (normalized === 'Draft') return 0;
  if (normalized === 'PendingManagerApproval') return 1;
  if (normalized === 'PendingFinanceReview') return 2;
  if (normalized === 'Approved') return 4;
  if (normalized === 'Rejected') return 5;
  if (normalized === 'Cancelled') return 7;
  return Number(value ?? 0);
}
