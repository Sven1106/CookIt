import { RecipeDetailComponent } from './recipeDetail/recipeDetail.component';

import { AlertService } from 'src/app/_services/alert.service';
import { Recipe } from 'src/app/_models/recipe';
import { IngredientWithIsMatchDto } from 'src/app/_models/ingredientWithIsMatchDto';

import { Component, OnInit, Inject } from '@angular/core';
import { RecipeSearchResultDto } from 'src/app/_models/recipeSearchResultDto';
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
  recipes: RecipeSearchResultDto[];
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
      this.recipes = JSON.parse(localStorage.getItem('recipesFound')).map((recipe: Recipe) => {
        const ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
        recipe.ingredients.forEach(ingredient => {
          const ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id === ingredient.id));
          ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
        });
        const recipeSearchResultDto = new RecipeSearchResultDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, placeholderImg, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
        return recipeSearchResultDto;
      });
      this.setTitle();
      this.lazyLoadImage();
    });



  }
  setTitle() {
    this.title = this.isDetailedView ? 'Detaljer om opskriften' : this.recipes.length + ' opskrifter fundet';
  }
  setCurrentScrollTop(event) {
    this.currentScrollTop = event.detail.scrollTop;
    const content = this.getContent();
    if (this.isDetailedView === true) {
      if (this.currentScrollTop < this.lockedScrollTop) {
        this.alertService.success('scrolling above content', 100);
        console.log('scrolling above content');
        content.scrollToPoint(0, this.lockedScrollTop);

      } else if (this.currentScrollTop > this.lockedScrollTop + this.currentSelectedRecipeContainerElement.clientHeight - content.clientHeight) {
        this.alertService.success('scrolling below content', 100);
        console.log('scrolling below content');
        content.scrollToPoint(0, this.lockedScrollTop + this.currentSelectedRecipeContainerElement.clientHeight - content.clientHeight);
      }
    }
  }

  toggleDetails(e: any, recipe: RecipeSearchResultDto) {
    this.currentSelectedRecipeContainerElement = e.currentTarget;
    const clientYOffset = this.currentSelectedRecipeContainerElement.offsetTop;
    console.log(clientYOffset);
    const clientXOffset = this.currentSelectedRecipeContainerElement.offsetLeft;
    const content = this.getContent();
    // const allRecipeElements = document.querySelectorAll('.recipeContainer');
    // allRecipeElements.forEach((recipeElement) => {
    //   if (recipeElement  !== this.currentSelectedRecipeContainerElement) {
    //     recipeElement.classList.toggle('hide');
    //   }
    // });
    // const selectedRecipeElement: HTMLElement = this.currentSelectedRecipeContainerElement.getElementsByClassName('recipe')[0] as HTMLElement;
    // if (this.currentSelectedRecipeContainerElement.classList.contains('detailedView')) {
    //   this.isDetailedView = false;
    //   // content.classList.remove('disableScroll');
    //   selectedRecipeElement.style.top = '0px';
    //   selectedRecipeElement.style.left = '0px';
    //   selectedRecipeElement.style.width = '100%';

    // } else {
    //   this.isDetailedView = true;
    //   selectedRecipeElement.style.top = (this.currentScrollTop - clientYOffset) + 'px';
    //   selectedRecipeElement.style.left = '-' + clientXOffset + 'px';
    //   selectedRecipeElement.style.width = '100vw';
    //   // content.classList.add('disableScroll');
    // }
    // this.lockedScrollTop = this.currentScrollTop;

    // this.setTitle();
    // this.currentSelectedRecipeContainerElement.classList.toggle('detailedView');
    const dialogRef = this.dialog.open(RecipeDetailComponent, {
      data: recipe,
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.animal = result;
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
                case 400:
                  console.log('E-mail eller Password er forkert');
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
    });
  }
}
