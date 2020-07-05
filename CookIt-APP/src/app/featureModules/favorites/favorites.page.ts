import { RecipeService } from '@core/services/recipe/recipe.service';
import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ImageService } from '@core/services/image/image.service';
import { ImageRequest } from '@core/models/imageRequest';
import { Platform } from '@ionic/angular';
import { RecipeWithMatchedIngredients } from '@core/models/recipeWithMatchedIngredients';
import { IngredientWithIsMatchDto } from '@core/models/ingredientWithIsMatchDto';
import { switchMap, flatMap } from 'rxjs/operators';
import { RecipeDetailComponent } from '@shared/components/recipeDetail/recipeDetail.component';
import { MatDialog } from '@angular/material/dialog';
import { RecipeForListDto } from '@core/models/recipeForListDto';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.page.html',
  styleUrls: ['./favorites.page.scss'],
})
export class FavoritesPage implements OnInit {

  favoriteRecipes: RecipeForListDto[] = [];
  constructor(
    private recipeService: RecipeService,
    private imageService: ImageService,
    private platform: Platform,
    public dialog: MatDialog
  ) {


  }
  ionViewWillEnter() {

  }
  ngOnInit() {
    const thumbNailRequest: ImageRequest = new ImageRequest('');
    thumbNailRequest.width = this.platform.width();
    thumbNailRequest.height = Math.floor(this.platform.width() * 0.75);
    this.imageService.getImage(thumbNailRequest).subscribe((placeholderImg) => {
      this.recipeService.favoriteRecipes.subscribe((favoriteRecipes) => {
        if (favoriteRecipes != null) {
          this.favoriteRecipes = favoriteRecipes.map((recipe: RecipeWithMatchedIngredients) => {
            const ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
            recipe.ingredients.forEach(ingredient => {
              const ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id === ingredient.id));
              ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
            });
            const recipeSearchResultDto = new RecipeForListDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, placeholderImg, recipe.isFavorite, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
            return recipeSearchResultDto;
          });
          this.lazyLoadImage();
        }
      });
    });
  }

  toggleDetails(recipe: RecipeForListDto) {
    console.log(recipe);
    const dialogRef = this.dialog.open(RecipeDetailComponent, {
      panelClass: 'custom-dialog-container',
      data: recipe,
    });
    dialogRef.afterClosed().subscribe(() => {
      if (dialogRef.componentInstance.oldIsFavorite !== recipe.isFavorite) {
        this.recipeService.toggleFavoriteRecipe(recipe);
      }
    });
  }


  prepareImageRequest(url: string = '') {
    const imageRequest: ImageRequest = new ImageRequest(url);
    imageRequest.width = Math.floor(this.platform.width() * 0.36);
    imageRequest.height = Math.floor(imageRequest.width * 0.75);
    return imageRequest;
  }

  lazyLoadImage() {
    if (this.favoriteRecipes != null) {
      this.favoriteRecipes.forEach((recipe, index) => {
        const imageRequest: ImageRequest = this.prepareImageRequest(recipe.imageUrl);
        this.imageService.getImage(imageRequest)
          .subscribe({
            next: (result: string) => {
              recipe.imageSrc = result;
              this.favoriteRecipes.splice(index, 1, recipe);
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
              // console.log('complete');
            }
          });
      });
    }
  }
}
