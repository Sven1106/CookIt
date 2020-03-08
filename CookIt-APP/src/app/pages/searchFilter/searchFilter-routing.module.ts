import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { SearchFilterPage } from './searchFilter.page';

const routes: Routes = [
  {
    path: '',
    component: SearchFilterPage
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SearchFilterPageRoutingModule {}
