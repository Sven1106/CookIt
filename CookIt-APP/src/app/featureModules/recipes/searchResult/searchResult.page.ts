
import { Component, OnInit, Inject } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { MatDialog } from '@angular/material/dialog';
import { Platform } from '@ionic/angular';
import { Router } from '@angular/router';
import { RecipeDetailComponent } from '@shared/components/recipeDetail/recipeDetail.component';
import { RecipeService } from '@core/services/recipe/recipe.service';
import { ImageService } from '@core/services/image/image.service';
import { RecipeWithMatchedIngredients } from '@core/models/recipeWithMatchedIngredients';
import { IngredientWithIsMatchDto } from '@core/models/ingredientWithIsMatchDto';
import { ImageRequest } from '@core/models/imageRequest';
import { RecipeForListDto } from '@core/models/recipeForListDto';



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
    private platform: Platform,
    private imageService: ImageService,
    private recipeService: RecipeService,
    private router: Router
  ) { }

  ngOnInit() {
    const thumbNailRequest: ImageRequest = new ImageRequest('');
    thumbNailRequest.width = this.platform.width();
    thumbNailRequest.height = Math.floor(this.platform.width() * 0.75);
    this.imageService.getImage(thumbNailRequest).subscribe((placeholderImg) => {
      this.recipeService.searchRecipes.subscribe(x => {
        if (x.length === 0) {
          this.router.navigateByUrl('tabs/recipes/searchFilter');
        }
        this.recipes = x.map((recipe: RecipeWithMatchedIngredients) => {
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
    });
  }

  setTitle() {
    this.title = this.isDetailedView ? 'Detaljer om opskriften' : this.recipes.length + ' opskrifter fundet';
  }

  toggleDetails(recipe: RecipeForListDto) {
    const dialogRef = this.dialog.open(RecipeDetailComponent, {
      panelClass: 'custom-dialog-container',
      data: recipe,
    });
    dialogRef.afterClosed().subscribe((data) => {
      if (dialogRef.componentInstance.oldIsFavorite !== recipe.isFavorite) {
        this.recipeService.toggleFavoriteRecipe(recipe);
      }
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
            // console.log('complete');
          }
        });
    });
  }
}
