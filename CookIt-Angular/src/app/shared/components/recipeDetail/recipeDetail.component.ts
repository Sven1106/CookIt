import { Component, Inject, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { RecipeService } from '@core/services/recipe/recipe.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RecipeForListDto } from '@core/models/recipeForListDto';

@Component({
  selector: 'app-recipeDetail',
  templateUrl: './recipeDetail.component.html',
  styleUrls: ['./recipeDetail.component.scss']
})

export class RecipeDetailComponent implements OnInit {

  oldIsFavorite: boolean;
  constructor(
    public dialogRef: MatDialogRef<RecipeDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public recipe: RecipeForListDto,
    public recipeService: RecipeService
  ) {
    this.oldIsFavorite = this.recipe.isFavorite;
  }
  ngOnInit() {
    this.dialogRef.updateSize('90vw');
    this.dialogRef.addPanelClass('custom-dialog-container');
    this.dialogRef.updatePosition({
      top: '25vh'
    });


  }

  goToUrl(event: string) {
    window.open(event, '_system');
  }

  toggleFavorite(recipe: RecipeForListDto) {
    this.recipe.isFavorite = !this.recipe.isFavorite;
  }
}