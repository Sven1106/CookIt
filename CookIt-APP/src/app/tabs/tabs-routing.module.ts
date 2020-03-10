import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TabsPage } from './tabs.page';

const routes: Routes = [
  {
    path: 'tabs',
    component: TabsPage,
    children: [
      {
        path: 'favorites',
        loadChildren: () => import('../pages/favorites/favorites.module').then(m => m.FavoritesPageModule)
      },
      {
        path: 'recipes',
        children: [
          { path: '', redirectTo: 'searchfilter', pathMatch: 'full' },
          {
            path: 'searchfilter',
            loadChildren: () => import('../pages/searchFilter/searchFilter.module').then(m => m.SearchFilterPageModule)
          },
          {
            path: 'searchResult',
            loadChildren: () => import('../pages/searchResult/searchResult.module').then( m => m.SearchresultPageModule)
          }
        ]

      },
      {
        path: 'user',
        loadChildren: () =>
          import('../pages/user/user.module').then(m => m.UserPageModule)
      },
      {
        path: '',
        redirectTo: 'recipes',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '',
    redirectTo: 'tabs',
    pathMatch: 'full'
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TabsPageRoutingModule { }
