import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { RecipeService } from 'src/app/_services/recipe.service';
import { RecipeForListDto } from 'src/app/_models/recipeSearchResultDto';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-recipeDetail',
  templateUrl: './recipeDetail.component.html',
  styleUrls: ['./recipeDetail.component.scss']
})

export class RecipeDetailComponent implements OnInit {

  constructor(
    public dialogRef: MatDialogRef<RecipeDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public recipe: RecipeForListDto,
    private recipeService: RecipeService
  ) {


  }
  ngOnInit() {
    this.dialogRef.updateSize('90vw');
    this.dialogRef.addPanelClass('custom-dialog-container');
    this.dialogRef.updatePosition({
      top: '25vh'
    });


  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  goToUrl(event: string) {
    window.open(event, '_system');
  }

  toggleFavorite(recipe: RecipeForListDto) {
    this.recipeService.toggleFavoriteRecipe(recipe.id)
      .subscribe({
        next: (result: any) => {
          recipe.isFavorite = !recipe.isFavorite;
        },
        error: (error: any) => {
          console.error(error);
          if (error instanceof HttpErrorResponse) {
            switch (error.status) {
              case 400:
                console.log('Ingen opskrifter fundet eller ingen ændringer lavet');
                break;
              case 0:
                console.log('Ingen forbindelse til serveren');
                break;
              default:
                const applicationError = error.headers.get('Application-Error');
                if (applicationError) {
                  console.log(applicationError);
                }
                else {
                  console.log('Der opstod en fejl');
                }
                break;
            }
          }
        },
        complete: () => {
          // console.log("complete");
        }
      });
  }
}