import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RequestDetailPageComponent } from './request-detail-page.component';
import { RequestsPageComponent } from './requests-page.component';

const routes: Routes = [
  {
    path: '',
    component: RequestsPageComponent
  },
  {
    path: ':id',
    component: RequestDetailPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RequestsRoutingModule {
}
