import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TabsComponent } from './tabs.component';

const routes: Routes = [
  {
    path: 'tabs',
    component: TabsComponent,
    children: [
      {
        path: 'favorites',
        loadChildren: () => import('@featureModules/favorites/favorites.module').then(m => m.FavoritesPageModule)
      },
      {
        path: 'recipes',
        loadChildren: () => import('@featureModules/recipes/recipes.module').then(m => m.RecipesModule)
      },
      {
        path: 'user',
        loadChildren: () =>
          import('@featureModules/user/user.module').then(m => m.UserPageModule)
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
