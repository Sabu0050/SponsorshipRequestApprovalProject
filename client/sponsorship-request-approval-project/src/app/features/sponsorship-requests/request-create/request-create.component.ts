import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { of, switchMap } from 'rxjs';
import { SponsorshipType } from '../../../core/models/sponsorship-type.models';
import { SaveSponsorshipRequestPayload } from '../../../core/models/sponsorship-request.models';
import { SponsorshipRequestService } from '../../../core/services/sponsorship-request.service';
import { SponsorshipTypeService } from '../../../core/services/sponsorship-type.service';
import { WorkflowService } from '../../../core/services/workflow.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-request-create',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './request-create.component.html',
  styles: [`
    .embedded-form {
      max-width: 100%;
      grid-template-columns: repeat(2, minmax(0, 1fr));
      gap: 12px;
    }

    .section-title {
      grid-column: 1 / -1;
      margin-top: 6px;
      padding-top: 6px;
      border-bottom: 1px solid #e2e8f0;
      font-size: .83rem;
      font-weight: 700;
      text-transform: uppercase;
      color: #475569;
      letter-spacing: 0;
    }

    .field {
      display: grid;
      gap: 6px;
    }

    .field.full {
      grid-column: 1 / -1;
    }

    .actions {
      grid-column: 1 / -1;
    }

    .error-text {
      color: #dc2626;
      font-size: .82rem;
    }

    @media (max-width: 900px) {
      .embedded-form {
        grid-template-columns: 1fr;
      }

      .field.full {
        grid-column: auto;
      }
    }
  `]
})
export class RequestCreateComponent implements OnInit {
  @Input() embedded = false;
  @Output() saved = new EventEmitter<void>();
  sponsorshipTypes: SponsorshipType[] = [];
  message = '';
  saving = false;
  readonly minEventDate = this.formatDateForInput(this.getTomorrow());
  readonly maxEventDate = this.formatDateForInput(this.getEighteenMonthsFromToday());
  readonly form = this.fb.group({
    title: ['', Validators.required], requestorName: ['', Validators.required], department: ['', Validators.required],
    sponsorshipTypeId: ['', Validators.required], eventOrganizationName: ['', Validators.required], eventDate: ['', [Validators.required, this.eventDateRangeValidator()]],
    requestedAmount: [0, [Validators.required, Validators.min(1)]], purpose: ['', Validators.required], expectedBusinessBenefit: [''], remarks: ['']
  });

  constructor(
    private readonly fb: FormBuilder,
    private readonly typeService: SponsorshipTypeService,
    private readonly requestService: SponsorshipRequestService,
    private readonly workflowService: WorkflowService,
    private readonly router: Router,
    private readonly authService: AuthService
  ) {}

  ngOnInit(): void {
    const requestorDisplay = this.authService.getDisplayName() || this.authService.currentUser()?.email || '';
    this.form.patchValue({ requestorName: requestorDisplay });
    this.form.controls.requestorName.disable({ emitEvent: false });

    this.typeService.getAll().subscribe({
      next: types => {
        this.sponsorshipTypes = types.filter(type => type.isActive);
        if (!this.sponsorshipTypes.length) {
          this.message = 'No sponsorship types returned from backend. Please verify GET /api/sponsorship-types data.';
        }
      },
      error: () => {
        this.message = 'Failed to load sponsorship types from backend.';
      }
    });
  }

  save(submit = false): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.saving = true;
    this.message = '';

    const formValue = this.form.getRawValue();
    const payload: SaveSponsorshipRequestPayload = {
      title: formValue.title ?? '',
      description: this.buildDescription(),
      sponsorshipTypeId: formValue.sponsorshipTypeId ?? '',
      sponsorName: formValue.eventOrganizationName ?? '',
      requestedAmount: Number(formValue.requestedAmount ?? 0),
      currencyCode: 'USD',
      eventDate: formValue.eventDate ?? null,
      sponsorshipStartDate: formValue.eventDate ?? null,
      sponsorshipEndDate: formValue.eventDate ?? null
    };

    const create$ = this.requestService.create(payload);
    const flow$ = submit
      ? create$.pipe(
        switchMap(created => {
          const requestId = this.extractCreatedRequestId(created);
          if (!requestId) {
            throw new Error('Create succeeded but request ID was not returned by API response.');
          }
          return this.workflowService.submit(requestId, { comments: formValue.remarks ?? '' });
        })
      )
      : create$.pipe(switchMap(() => of(null)));

    flow$.subscribe({
      next: () => {
        this.saving = false;
        this.message = submit ? 'Request submitted successfully.' : 'Draft saved successfully.';
        this.saved.emit();
        this.form.reset({
          title: '',
          requestorName: '',
          department: '',
          sponsorshipTypeId: '',
          eventOrganizationName: '',
          eventDate: '',
          requestedAmount: 0,
          purpose: '',
          expectedBusinessBenefit: '',
          remarks: ''
        });
        if (!this.embedded) {
          this.router.navigate(['/my-requests']);
        }
      },
      error: (error) => {
        this.saving = false;
        this.message = error?.error?.message ?? error?.message ?? 'Unable to save request.';
      }
    });
  }

  private extractCreatedRequestId(created: unknown): string | null {
    const response = created as
      | { id?: string; data?: { id?: string }; result?: { id?: string } }
      | null
      | undefined;
    return response?.id ?? response?.data?.id ?? response?.result?.id ?? null;
  }

  private buildDescription(): string {
    const formValue = this.form.getRawValue();
    return [
      `Requestor Name: ${formValue.requestorName ?? ''}`,
      `Department: ${formValue.department ?? ''}`,
      `Purpose: ${formValue.purpose ?? ''}`,
      `Expected Business Benefit: ${formValue.expectedBusinessBenefit ?? ''}`,
      `Remarks: ${formValue.remarks ?? ''}`
    ].join('\n');
  }

  isInvalid(controlName: keyof typeof this.form.controls): boolean {
    const control = this.form.controls[controlName];
    return !!(control.touched && control.invalid);
  }

  hasError(controlName: keyof typeof this.form.controls, errorName: string): boolean {
    const control = this.form.controls[controlName];
    return !!(control.touched && control.hasError(errorName));
  }

  private eventDateRangeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value as string | null | undefined;
      if (!value) {
        return null;
      }

      const selectedDate = new Date(value);
      if (Number.isNaN(selectedDate.getTime())) {
        return { invalidDate: true };
      }

      const today = new Date();
      today.setHours(0, 0, 0, 0);

      const minDate = new Date(today);
      minDate.setDate(minDate.getDate() + 1);

      const maxDate = new Date(today);
      maxDate.setMonth(maxDate.getMonth() + 18);

      if (selectedDate < minDate || selectedDate > maxDate) {
        return { dateOutOfRange: true };
      }

      return null;
    };
  }

  private getTomorrow(): Date {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    return tomorrow;
  }

  private getEighteenMonthsFromToday(): Date {
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const maxDate = new Date(today);
    maxDate.setMonth(maxDate.getMonth() + 18);
    return maxDate;
  }

  private formatDateForInput(date: Date): string {
    const year = date.getFullYear();
    const month = `${date.getMonth() + 1}`.padStart(2, '0');
    const day = `${date.getDate()}`.padStart(2, '0');
    return `${year}-${month}-${day}`;
  }
}
