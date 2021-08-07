import { IonicModule } from '@ionic/angular';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '@shared/shared.module';
import { RecipeRoutingModule } from './recipes-routing.module';
import { SearchResultPage } from './searchResult/searchResult.page';
import { SearchFilterPage } from './searchFilter/searchFilter.page';


@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    IonicModule,
    RecipeRoutingModule,
    SharedModule
  ],
  declarations: [SearchFilterPage, SearchResultPage]
})
export class RecipesModule { }
