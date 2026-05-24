import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  template: `<p>{{ text }}</p>`
})
export class LoadingSpinnerComponent {
  @Input() text = 'Loading...';
}
