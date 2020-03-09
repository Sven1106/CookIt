import { RecipeSearchResultDto } from './../../../_models/recipeSearchResultDto';
import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-recipeDetail',
  templateUrl: './recipeDetail.component.html',
  styleUrls: ['./recipeDetail.component.scss']
})

export class RecipeDetailComponent {

  constructor(
    public dialogRef: MatDialogRef<RecipeDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public recipe: RecipeSearchResultDto
  ) {

  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  goToUrl(event: string) {
    window.open(event, '_system');
  }
}