import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ApprovalsRoutingModule } from './approvals-routing.module';
import { ApprovalsPageComponent } from './approvals-page.component';

@NgModule({
  declarations: [ApprovalsPageComponent],
  imports: [
    CommonModule,
    ApprovalsRoutingModule
  ]
})
export class ApprovalsModule {
}
