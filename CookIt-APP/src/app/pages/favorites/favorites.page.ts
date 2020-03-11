
import { RecipeForListDto } from 'src/app/_models/RecipeForListDto';
import { RecipeService } from 'src/app/_services/recipe.service';
import { Component, OnInit } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { AlertService } from 'src/app/_services/alert.service';
import { ImageService } from 'src/app/_services/image.service';
import { ImageRequest } from 'src/app/_models/imageRequest';
import { Platform } from '@ionic/angular';
import { RecipeWithMatchedIngredients } from 'src/app/_models/recipeWithMatchedIngredients';
import { IngredientWithIsMatchDto } from 'src/app/_models/ingredientWithIsMatchDto';
import { switchMap, flatMap } from 'rxjs/operators';
import { MatDialog } from '@angular/material';
import { RecipeDetailComponent } from 'src/app/_shared/recipeDetail/recipeDetail.component';

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
    this.recipeService.setAllFavoriteRecipesWithMatchedIngredients();
  }
  ionViewWillEnter() {
    this.GetAllFavoriteRecipesWithPlaceholderImage();
  }

  private GetAllFavoriteRecipesWithPlaceholderImage() {

    const thumbNailRequest: ImageRequest = this.prepareImageRequest();
    if (this.recipeService.favoriteRecipes == null) {
      return this.favoriteRecipes = [];
    }
    this.favoriteRecipes = this.recipeService.favoriteRecipes.map((recipe: RecipeWithMatchedIngredients) => {
      const ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
      recipe.ingredients.forEach(ingredient => {
        console.log(ingredient);
        const ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id === ingredient.id));
        ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
      });
      const recipeSearchResultDto = new RecipeForListDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, '', recipe.isFavorite, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
      return recipeSearchResultDto;
    });
    // this.recipeService.getIngredientsFromKitchenCupboardInStorage().then(ingredients => {
    //   this.imageService.getImage(thumbNailRequest).pipe(
    //     switchMap(placeholderImg => {
    //       return this.recipeService.getAllFavoriteRecipesWithIngredientMatches(ingredients).pipe(
    //         switchMap(recipeWithMatchIngredients => {

    //           console.log(recipeWithMatchIngredients);
    //           console.log(this.favoriteRecipes);
    //           if (recipeWithMatchIngredients == null) {
    //             return this.favoriteRecipes = [];
    //           }
    //           return this.favoriteRecipes = recipeWithMatchIngredients.map((recipe: RecipeWithMatchedIngredients) => {
    //             const ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
    //             recipe.ingredients.forEach(ingredient => {
    //               const ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id === ingredient.id));
    //               ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
    //             });
    //             const recipeSearchResultDto = new RecipeForListDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, placeholderImg, recipe.isFavorite, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
    //             return recipeSearchResultDto;
    //           });
    //         })
    //       );
    //     })
    //   ).subscribe(() => {
    //     setTimeout(() => {
    //       this.lazyLoadImage();
    //     }, 5000);
    //   });
    // });
  }

  ngOnInit() {

  }

  toggleDetails(recipe: RecipeForListDto) {
    console.log(recipe);
    const dialogRef = this.dialog.open(RecipeDetailComponent, {
      panelClass: 'custom-dialog-container',
      data: recipe,
    });
    dialogRef.afterClosed().subscribe(result => {
      this.GetAllFavoriteRecipesWithPlaceholderImage();
    });
  }


  prepareImageRequest(url: string = '') {
    const imageRequest: ImageRequest = new ImageRequest(url);
    imageRequest.width = Math.floor(this.platform.width() * 0.36);
    imageRequest.height = Math.floor(imageRequest.width * 0.75);
    return imageRequest;
  }

  lazyLoadImage() {
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
            // console.log("complete");
          }
        });
    });
  }
}
