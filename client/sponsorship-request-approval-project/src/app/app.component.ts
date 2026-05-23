import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { NavigationService } from './core/services/navigation.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  protected readonly authService: AuthService;
  protected readonly navigationService: NavigationService;

  constructor(authService: AuthService, navigationService: NavigationService) {
    this.authService = authService;
    this.navigationService = navigationService;
  }

  protected logout(): void {
    this.authService.logout();
  }
}
