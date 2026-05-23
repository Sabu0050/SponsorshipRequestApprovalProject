import { Component, OnInit } from '@angular/core';
import { AdminService, Role } from '../../core/services/admin.service';

@Component({
  selector: 'app-admin-page',
  standalone: false,
  templateUrl: './admin-page.component.html'
})
export class AdminPageComponent implements OnInit {
  protected roles: Role[] = [];

  constructor(private readonly adminService: AdminService) {
  }

  ngOnInit(): void {
    this.adminService.getRoles().subscribe(result => this.roles = result);
  }
}
