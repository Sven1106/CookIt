import { RecipeDetailComponent } from './recipeDetail/recipeDetail.component';

import { AlertService } from 'src/app/_services/alert.service';
import { RecipeWithMatchedIngredients } from 'src/app/_models/recipe';
import { IngredientWithIsMatchDto } from 'src/app/_models/ingredientWithIsMatchDto';

import { Component, OnInit, Inject } from '@angular/core';
import { RecipeForListDto } from 'src/app/_models/recipeSearchResultDto';
import { MatDialog } from '@angular/material/dialog';
import { ImageRequest } from 'src/app/_models/imageRequest';
import { ImageService } from 'src/app/_services/image.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Platform } from '@ionic/angular';



@Component({
  selector: 'app-searchResult',
  templateUrl: './searchResult.page.html',
  styleUrls: ['./searchResult.page.scss'],
})
export class SearchResultPage implements OnInit {
  animal: string;
  name: string;
  title: string;
  currentScrollTop: number;
  lockedScrollTop: number;
  isDetailedView = false;
  recipes: RecipeForListDto[];
  currentSelectedRecipeContainerElement: HTMLElement;
  constructor(
    public dialog: MatDialog,
    private alertService: AlertService,
    private imageService: ImageService,
    private platform: Platform
  ) { }

  ngOnInit() {

    const thumbNailRequest: ImageRequest = new ImageRequest('');
    thumbNailRequest.width = this.platform.width();
    thumbNailRequest.height = Math.floor(this.platform.width() * 0.75);
    this.imageService.getImage(thumbNailRequest).subscribe((placeholderImg) => {
      this.recipes = JSON.parse(localStorage.getItem('recipesFound')).map((recipe: RecipeWithMatchedIngredients) => {
        const ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
        recipe.ingredients.forEach(ingredient => {
          const ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id === ingredient.id));
          ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
        });
        const recipeSearchResultDto = new RecipeForListDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, placeholderImg, recipe.isFavorite, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
        return recipeSearchResultDto;
      });
      this.setTitle();
      this.lazyLoadImage();
    });



  }
  setTitle() {
    this.title = this.isDetailedView ? 'Detaljer om opskriften' : this.recipes.length + ' opskrifter fundet';
  }

  toggleDetails(e: any, recipe: RecipeForListDto) {
    const dialogRef = this.dialog.open(RecipeDetailComponent, {
      panelClass: 'custom-dialog-container',
      data: recipe,
    });
    dialogRef.afterClosed().subscribe(result => {

    });


  }
  getContent() {
    return document.querySelector('ion-content');
  }


  lazyLoadImage() {
    this.recipes.forEach((recipe, index) => {
      const imageRequest: ImageRequest = new ImageRequest(recipe.imageUrl);
      imageRequest.width = this.platform.width();
      imageRequest.height = Math.floor(this.platform.width() * 0.75);

      this.imageService.getImage(imageRequest)
        .subscribe({
          next: (result: string) => {
            recipe.imageSrc = result;
            this.recipes.splice(index, 1, recipe);
          },
          error: (error: any) => {
            console.error(error);
            if (error instanceof HttpErrorResponse) {
              switch (error.status) {
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
    });
  }
}
