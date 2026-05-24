import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-confirmation-dialog',
  standalone: true,
  template: `
    @if (open) {
      <div class="overlay">
        <div class="dialog">
          <h4>{{ title }}</h4>
          <p>{{ message }}</p>
          <div class="actions">
            <button type="button" (click)="cancel.emit()">Cancel</button>
            <button type="button" (click)="confirm.emit()">Confirm</button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .overlay { position: fixed; inset: 0; background: rgba(0,0,0,.2); display: grid; place-items: center; }
    .dialog { background: #fff; padding: 16px; border-radius: 8px; min-width: 280px; }
    .actions { display: flex; justify-content: flex-end; gap: 8px; }
  `]
})
export class ConfirmationDialogComponent {
  @Input() open = false;
  @Input() title = 'Confirm';
  @Input() message = 'Are you sure?';
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
}
