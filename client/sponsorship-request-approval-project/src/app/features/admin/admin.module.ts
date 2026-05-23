import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminPageComponent } from './admin-page.component';

@NgModule({
  declarations: [AdminPageComponent],
  imports: [
    CommonModule,
    AdminRoutingModule
  ]
})
export class AdminModule {
}
