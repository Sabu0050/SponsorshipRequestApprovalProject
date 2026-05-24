import { Component, HostListener } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { SidebarComponent } from './shared/components/sidebar/sidebar.component';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, NavbarComponent, SidebarComponent, ToastModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  protected readonly authService: AuthService;
  sidebarOpen = true;
  isMobile = false;

  constructor(authService: AuthService) {
    this.authService = authService;
    this.syncViewport();
  }

  @HostListener('window:resize')
  onResize(): void {
    this.syncViewport();
  }

  toggleSidebar(): void {
    this.sidebarOpen = !this.sidebarOpen;
  }

  closeSidebarOnMobile(): void {
    if (this.isMobile) {
      this.sidebarOpen = false;
    }
  }

  private syncViewport(): void {
    this.isMobile = window.innerWidth <= 992;
    if (this.isMobile) {
      this.sidebarOpen = false;
      return;
    }

    this.sidebarOpen = true;
  }
}
