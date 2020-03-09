import { RecipeDetailComponent } from './recipeDetail/recipeDetail.component';

import { AlertService } from 'src/app/_services/alert.service';
import { Recipe } from 'src/app/_models/recipe';
import { IngredientWithIsMatchDto } from 'src/app/_models/ingredientWithIsMatchDto';

import { Component, OnInit, Inject } from '@angular/core';
import { RecipeSearchResultDto } from 'src/app/_models/recipeSearchResultDto';
import { MatDialog } from '@angular/material/dialog';



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
    private alertService: AlertService,
    public dialog: MatDialog
  ) { }

  ngOnInit() {
    this.recipes = JSON.parse(localStorage.getItem('recipesFound')).map((recipe: Recipe) => {
      const ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
      recipe.ingredients.forEach(ingredient => {
        const ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id == ingredient.id));
        ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
      });
      const recipeSearchResultDto = new RecipeSearchResultDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
      return recipeSearchResultDto;
    });
    this.setTitle();

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
      width: '90vw',
      data: recipe,
    });

    // dialogRef.updatePosition({
    //   top: `200px`
    // });

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
      this.animal = result;
    });


  }
  getContent() {
    return document.querySelector('ion-content');
  }

}
