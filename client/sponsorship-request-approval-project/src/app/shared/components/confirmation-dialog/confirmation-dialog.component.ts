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
    .overlay { position: fixed; inset: 0; background: rgba(0,0,0,.2); display: grid; place-items: center; padding: 12px; z-index: 60; }
    .dialog { background: #fff; padding: 16px; border-radius: 8px; width: min(460px, 100%); max-height: calc(100vh - 24px); overflow: auto; }
    .actions { display: flex; justify-content: flex-end; gap: 8px; flex-wrap: wrap; }
    @media (max-width: 640px) {
      .actions button { width: 100%; }
    }
  `]
})
export class ConfirmationDialogComponent {
  @Input() open = false;
  @Input() title = 'Confirm';
  @Input() message = 'Are you sure?';
  @Output() confirm = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
}
