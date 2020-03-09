import { RecipeDetailComponent } from './recipeDetail/recipeDetail.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { IonicModule } from '@ionic/angular';

import { SearchResultPageRoutingModule } from './searchResult-routing.module';
import { SearchResultPage } from './searchResult.page';
import { SharedFormsModule } from 'src/app/_shared/sharedForms.module';
import { MatDialogModule } from '@angular/material';
@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    IonicModule,
    SearchResultPageRoutingModule,
    SharedFormsModule
  ],
  declarations: [SearchResultPage, RecipeDetailComponent],
  entryComponents: [RecipeDetailComponent]
})
export class SearchresultPageModule { }
