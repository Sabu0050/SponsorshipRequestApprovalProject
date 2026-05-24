import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginOption } from '../../../core/models/auth.models';
import { CardModule } from 'primeng/card';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule, CardModule, InputTextModule, PasswordModule, ButtonModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  readonly defaultDemoPassword = 'Demo@12345';
  rolePresets: LoginOption[] = [];
  selectedRole = '';

  error = '';
  loading = false;
  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]]
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly route: ActivatedRoute
  ) {
    this.authService.getLoginOptions().subscribe({
      next: options => {
        this.rolePresets = options ?? [];
        if (this.rolePresets.length > 0) {
          this.selectRolePreset(this.rolePresets[0].role);
        }
      },
      error: () => {
        this.rolePresets = [];
      }
    });
  }

  selectRolePreset(role: string): void {
    this.selectedRole = role;
    const selected = this.rolePresets.find(item => item.role === role);
    this.form.patchValue({
      email: selected?.email ?? '',
      password: this.defaultDemoPassword
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.error = '';
    this.authService.login(this.form.getRawValue() as { email: string; password: string }).subscribe({
      next: () => this.router.navigateByUrl(this.route.snapshot.queryParamMap.get('returnUrl') ?? this.authService.getLandingRoute()),
      error: err => {
        this.error = err?.error?.message ?? 'Login failed.';
        this.loading = false;
      }
    });
  }
}
