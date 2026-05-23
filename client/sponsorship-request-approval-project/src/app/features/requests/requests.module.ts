import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RequestsRoutingModule } from './requests-routing.module';
import { RequestDetailPageComponent } from './request-detail-page.component';
import { RequestsPageComponent } from './requests-page.component';

@NgModule({
  declarations: [
    RequestDetailPageComponent,
    RequestsPageComponent
  ],
  imports: [
    CommonModule,
    RequestsRoutingModule
  ]
})
export class RequestsModule {
}
