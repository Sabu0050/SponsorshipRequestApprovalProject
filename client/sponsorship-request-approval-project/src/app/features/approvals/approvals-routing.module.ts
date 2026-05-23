import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ApprovalsPageComponent } from './approvals-page.component';

const routes: Routes = [
  {
    path: '',
    component: ApprovalsPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ApprovalsRoutingModule {
}
