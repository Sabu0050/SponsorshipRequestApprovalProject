import { Component } from '@angular/core';
import { CardModule } from 'primeng/card';

@Component({
  selector: 'app-request-edit',
  standalone: true,
  imports: [CardModule],
  template: `
    <p-card header="Edit Request">
      <p>The current backend controller does not expose an update endpoint for sponsorship requests.</p>
    </p-card>
  `
})
export class RequestEditComponent {}
