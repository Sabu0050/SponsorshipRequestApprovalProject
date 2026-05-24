import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SponsorshipRequestListItem, WorkflowHistory } from '../../../core/models/sponsorship-request.models';
import { SponsorshipRequestService } from '../../../core/services/sponsorship-request.service';
import { SponsorshipTypeService } from '../../../core/services/sponsorship-type.service';
import { WorkflowService } from '../../../core/services/workflow.service';
import { SponsorshipType } from '../../../core/models/sponsorship-type.models';
import { StatusBadgeComponent } from '../../../shared/components/status-badge/status-badge.component';
import { normalizeAction, normalizeStatus } from '../../../shared/utils/status.utils';
import { AdminRole, AdminService } from '../../../core/services/admin.service';
import { AuthService } from '../../../core/services/auth.service';

interface RequestSummary {
  total: number;
  draft: number;
  pendingManagerApproval: number;
  pendingFinanceReview: number;
  approved: number;
  rejected: number;
  cancelled: number;
}

@Component({
  selector: 'app-all-requests',
  standalone: true,
  imports: [FormsModule, StatusBadgeComponent, DecimalPipe, DatePipe],
  templateUrl: './all-requests.component.html',
  styleUrl: './all-requests.component.css'
})
export class AllRequestsComponent implements OnInit {
  @ViewChild('workflowHistorySection') workflowHistorySection?: ElementRef<HTMLElement>;
  protected readonly normalizeStatus = normalizeStatus;
  protected readonly normalizeAction = normalizeAction;
  items: SponsorshipRequestListItem[] = [];
  history: WorkflowHistory[] = [];
  selectedRequest?: SponsorshipRequestListItem;
  sponsorshipTypes: SponsorshipType[] = [];
  roles: AdminRole[] = [];
  users: Array<{
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    department?: string;
    role: string;
    canRequestorAccess: boolean;
    canApproveManagerStage: boolean;
    canApproveFinanceStage: boolean;
  }> = [];
  newTypeName = '';
  showRoleModal = false;
  editingRoleName = '';
  roleForm = {
    name: '',
    canRequestorAccess: false,
    canApproveManagerStage: false,
    canApproveFinanceStage: false
  };
  showUserModal = false;
  editingUserId = '';
  userForm = {
    email: '',
    password: '',
    firstName: '',
    lastName: '',
    department: '',
    role: ''
  };
  savingType = false;
  savingRole = false;
  savingUser = false;
  adminMessage = '';
  search = '';
  status = '';
  constructor(
    private readonly service: SponsorshipRequestService,
    private readonly workflowService: WorkflowService,
    private readonly sponsorshipTypeService: SponsorshipTypeService,
    private readonly adminService: AdminService,
    private readonly authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadRequests();
    this.sponsorshipTypeService.getAll().subscribe(types => this.sponsorshipTypes = types);
    this.adminService.getRoles().subscribe(roles => this.roles = roles);
    this.loadUsers();
  }

  loadRequests(): void {
    this.service.getAll(1, 100).subscribe(result => {
      this.items = result.items;
      if (!this.selectedRequest && this.items.length) {
        this.select(this.items[0]);
      }
    });
  }
  get summary(): RequestSummary {
    return {
      total: this.items.length,
      draft: this.items.filter(item => normalizeStatus(item.status) === 'Draft').length,
      pendingManagerApproval: this.items.filter(item => normalizeStatus(item.status) === 'PendingManagerApproval').length,
      pendingFinanceReview: this.items.filter(item => normalizeStatus(item.status) === 'PendingFinanceReview').length,
      approved: this.items.filter(item => normalizeStatus(item.status) === 'Approved').length,
      rejected: this.items.filter(item => normalizeStatus(item.status) === 'Rejected').length,
      cancelled: this.items.filter(item => normalizeStatus(item.status) === 'Cancelled').length
    };
  }
  get filtered(): SponsorshipRequestListItem[] {
    const term = this.search.toLowerCase();
    return this.items.filter(item => {
      const statusLabel = normalizeStatus(item.status);
      const matchesStatus = !this.status || statusLabel === this.status;
      const matchesText = !term || `${item.requestNumber} ${item.title} ${item.requesterName}`.toLowerCase().includes(term);
      return matchesStatus && matchesText;
    });
  }

  clearFilters(): void {
    this.search = '';
    this.status = '';
  }

  select(item: SponsorshipRequestListItem): void {
    this.selectedRequest = item;
    this.workflowService.getHistory(item.id, 1, 50).subscribe(result => {
      this.history = result.items;
      this.workflowHistorySection?.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
    });
  }

  addType(): void {
    if (!this.canManageAdminConfig) {
      this.adminMessage = 'Only SystemAdmin can add sponsorship types.';
      return;
    }

    const name = this.newTypeName.trim();
    if (!name || this.savingType) {
      return;
    }

    this.savingType = true;
    this.sponsorshipTypeService.create({ name, isActive: true }).subscribe({
      next: () => {
        this.newTypeName = '';
        this.adminMessage = 'Sponsorship type added successfully.';
        this.savingType = false;
        this.sponsorshipTypeService.getAll().subscribe(types => this.sponsorshipTypes = types);
      },
      error: (error) => {
        this.adminMessage = error?.error ?? error?.message ?? 'Unable to add sponsorship type.';
        this.savingType = false;
      }
    });
  }

  toggleType(type: SponsorshipType): void {
    if (this.savingType) {
      return;
    }

    this.savingType = true;
    this.sponsorshipTypeService.update(type.id, {
      name: type.name,
      description: '',
      isActive: !type.isActive
    }).subscribe({
      next: () => {
        this.savingType = false;
        this.sponsorshipTypeService.getAll().subscribe(types => this.sponsorshipTypes = types);
      },
      error: () => {
        this.savingType = false;
      }
    });
  }

  openCreateRoleModal(): void {
    this.editingRoleName = '';
    this.roleForm = {
      name: '',
      canRequestorAccess: false,
      canApproveManagerStage: false,
      canApproveFinanceStage: false
    };
    this.showRoleModal = true;
  }

  openEditRoleModal(role: AdminRole): void {
    this.adminService.getRoles().subscribe(freshRoles => {
      this.roles = freshRoles;
      const selected = freshRoles.find(item =>
        item.name.trim().toLowerCase() === role.name.trim().toLowerCase()) ?? role;
      this.editingRoleName = selected.name;
      this.roleForm = {
        name: selected.name,
        canRequestorAccess: selected.canRequestorAccess,
        canApproveManagerStage: selected.canApproveManagerStage,
        canApproveFinanceStage: selected.canApproveFinanceStage
      };
      this.onRoleFormNameChanged(this.roleForm.name);
      this.showRoleModal = true;
    });
  }

  onRoleFormNameChanged(roleName: string): void {
    const matchedRole = this.roles.find(role => role.name === roleName);
    if (!matchedRole) {
      return;
    }

    this.roleForm.canRequestorAccess = matchedRole.canRequestorAccess;
    this.roleForm.canApproveManagerStage = matchedRole.canApproveManagerStage;
    this.roleForm.canApproveFinanceStage = matchedRole.canApproveFinanceStage;
  }

  closeRoleModal(): void {
    this.showRoleModal = false;
    this.savingRole = false;
  }

  saveRole(): void {
    if (!this.canManageAdminConfig) {
      this.adminMessage = 'Only SystemAdmin can add roles.';
      return;
    }

    const name = this.roleForm.name.trim();
    if (!name || this.savingRole) {
      return;
    }

    this.savingRole = true;
    if (this.editingRoleName) {
      this.adminService.updateRoleAuthorities(this.editingRoleName, {
        canRequestorAccess: this.roleForm.canRequestorAccess,
        canApproveManagerStage: this.roleForm.canApproveManagerStage,
        canApproveFinanceStage: this.roleForm.canApproveFinanceStage
      }).subscribe({
        next: () => {
          this.adminMessage = 'Role authority updated successfully.';
          this.savingRole = false;
          this.adminService.getRoles().subscribe(roles => {
            this.roles = roles;
            this.closeRoleModal();
          });
          this.loadUsers();
        },
        error: (error) => {
          this.adminMessage = error?.error ?? error?.message ?? 'Unable to update role.';
          this.savingRole = false;
        }
      });
      return;
    }

    this.adminService.createRole({
      name,
      canRequestorAccess: this.roleForm.canRequestorAccess,
      canApproveManagerStage: this.roleForm.canApproveManagerStage,
      canApproveFinanceStage: this.roleForm.canApproveFinanceStage
    }).subscribe({
      next: () => {
        this.adminMessage = 'Role added successfully.';
        this.savingRole = false;
        this.adminService.getRoles().subscribe(roles => {
          this.roles = roles;
          this.closeRoleModal();
        });
      },
      error: (error) => {
        this.adminMessage = error?.error ?? error?.message ?? 'Unable to add role.';
        this.savingRole = false;
      }
    });
  }

  loadUsers(): void {
    this.adminService.getUsers().subscribe(users => this.users = users);
  }

  openCreateUserModal(): void {
    this.editingUserId = '';
    this.userForm = {
      email: '',
      password: '',
      firstName: '',
      lastName: '',
      department: '',
      role: this.roles[0]?.name ?? ''
    };
    this.showUserModal = true;
  }

  openEditUserModal(user: {
    id: string;
    email: string;
    firstName: string;
    lastName: string;
    department?: string;
    role: string;
    canRequestorAccess: boolean;
    canApproveManagerStage: boolean;
    canApproveFinanceStage: boolean;
  }): void {
    this.editingUserId = user.id;
    this.userForm = {
      email: user.email,
      password: '',
      firstName: user.firstName,
      lastName: user.lastName,
      department: user.department ?? '',
      role: user.role
    };
    this.showUserModal = true;
  }

  closeUserModal(): void {
    this.showUserModal = false;
    this.savingUser = false;
  }

  saveUser(): void {
    if (this.savingUser || !this.userForm.role) {
      return;
    }

    this.savingUser = true;

    if (this.editingUserId) {
      this.adminService.updateUserAuthorities(this.editingUserId, {
        firstName: this.userForm.firstName,
        lastName: this.userForm.lastName,
        department: this.userForm.department,
        role: this.userForm.role
      }).subscribe({
        next: () => {
          this.savingUser = false;
          this.closeUserModal();
          this.loadUsers();
        },
        error: () => {
          this.savingUser = false;
        }
      });
      return;
    }

    this.adminService.createUser({
      email: this.userForm.email,
      password: this.userForm.password,
      firstName: this.userForm.firstName,
      lastName: this.userForm.lastName,
      department: this.userForm.department,
      role: this.userForm.role
    }).subscribe({
      next: () => {
        this.savingUser = false;
        this.closeUserModal();
        this.loadUsers();
      },
      error: () => {
        this.savingUser = false;
      }
    });
  }

  get pendingApprovalCount(): number {
    return this.items.filter(item => {
      const status = normalizeStatus(item.status);
      return status === 'PendingManagerApproval' || status === 'PendingFinanceReview';
    }).length;
  }

  get activeTypesCount(): number {
    return this.sponsorshipTypes.filter(item => item.isActive).length;
  }

  get canManageAdminConfig(): boolean {
    return this.authService.hasRole('SystemAdmin');
  }

  get visibleRoles(): AdminRole[] {
    return this.roles.filter(role => role.name.trim().toLowerCase() !== 'systemadmin');
  }

}
