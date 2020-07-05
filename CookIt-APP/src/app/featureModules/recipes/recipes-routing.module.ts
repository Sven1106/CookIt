import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SearchResultPage } from './searchResult/searchResult.page';
import { SearchFilterPage } from './searchFilter/searchFilter.page';

const routes: Routes = [
  {
    path: 'searchfilter',
    component: SearchFilterPage
  },
  {
    path: 'searchResult',
    component: SearchResultPage
  },
  {
    path: '',
    redirectTo: 'searchfilter',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class RecipeRoutingModule {}
