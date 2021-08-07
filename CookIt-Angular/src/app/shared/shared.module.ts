
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from './material.module';
import { RecipeDetailComponent } from './components/recipeDetail/recipeDetail.component';
@NgModule({
  declarations: [RecipeDetailComponent],
  entryComponents: [RecipeDetailComponent],
  imports: [
    CommonModule,
    MaterialModule
  ],
  exports: [
    MaterialModule,
    RecipeDetailComponent
  ],
})
export class SharedModule { }
