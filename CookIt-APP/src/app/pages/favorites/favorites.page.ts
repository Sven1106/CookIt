
import { RecipeForListDto } from 'src/app/_models/recipeSearchResultDto';
import { RecipeService } from 'src/app/_services/recipe.service';
import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AlertService } from 'src/app/_services/alert.service';
import { ImageService } from 'src/app/_services/image.service';
import { ImageRequest } from 'src/app/_models/imageRequest';
import { Platform } from '@ionic/angular';
import { RecipeWithMatchedIngredients } from 'src/app/_models/recipe';
import { IngredientWithIsMatchDto } from 'src/app/_models/ingredientWithIsMatchDto';
import { switchMap, flatMap } from 'rxjs/operators';

@Component({
  selector: 'app-favorites',
  templateUrl: './favorites.page.html',
  styleUrls: ['./favorites.page.scss'],
})
export class FavoritesPage implements OnInit {

  favoriteRecipes: RecipeForListDto[];
  constructor(
    private recipeService: RecipeService,
    private alertService: AlertService,
    private imageService: ImageService,
    private platform: Platform
  ) { }

  ngOnInit() {
    const thumbNailRequest: ImageRequest = new ImageRequest('');
    thumbNailRequest.width = Math.floor(this.platform.width() * 0.30);
    thumbNailRequest.height = Math.floor(thumbNailRequest.width * 0.75);
    this.imageService.getImage(thumbNailRequest).pipe(
      flatMap(placeholderImg => {
        return this.recipeService.getAllFavoriteRecipes()
          .pipe(
            flatMap(recipeWithMatchIngredients => {
              return this.favoriteRecipes = recipeWithMatchIngredients.map((recipe: RecipeWithMatchedIngredients) => {
                const ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
                recipe.ingredients.forEach(ingredient => {
                  const ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id === ingredient.id));
                  ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
                });
                const recipeSearchResultDto = new RecipeForListDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, placeholderImg, recipe.isFavorite, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
                return recipeSearchResultDto;
              });
            }));
      })
    ).subscribe(() => this.lazyLoadImage());
  }
  lazyLoadImage() {
    this.favoriteRecipes.forEach((recipe, index) => {
      const imageRequest: ImageRequest = new ImageRequest(recipe.imageUrl);
      imageRequest.width = Math.floor(this.platform.width() * 0.36);
      imageRequest.height = Math.floor(imageRequest.width * 0.75);

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
            // console.log("complete");
          }
        });
    });
  }
}
