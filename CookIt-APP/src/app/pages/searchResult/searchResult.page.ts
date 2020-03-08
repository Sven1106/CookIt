import { map } from 'rxjs/operators';
import { Recipe } from 'src/app/_models/recipe';
import { IngredientWithIsMatchDto } from 'src/app/_models/ingredientWithIsMatchDto';

import { Component, OnInit } from '@angular/core';
import { RecipeSearchResultDto } from 'src/app/_models/recipeSearchResultDto';

@Component({
  selector: 'app-searchResult',
  templateUrl: './searchResult.page.html',
  styleUrls: ['./searchResult.page.scss'],
})
export class SearchResultPage implements OnInit {

  title: string;
  currentScrollTop: number = 0;
  lockedScrollTop: number = 0;
  isDetailedView = false;
  recipes: RecipeSearchResultDto[];
  currentSelectedRecipeContainerElement: HTMLElement;
  constructor(
  ) { }

  ngOnInit() {
    this.recipes = JSON.parse(localStorage.getItem('recipesFound')).map((recipe: Recipe) => {
      let ingredientsWithIsMatchDto: IngredientWithIsMatchDto[] = [];
      recipe.ingredients.forEach(ingredient => {
        let ingredientWithIsMatchDto: IngredientWithIsMatchDto = new IngredientWithIsMatchDto(ingredient.id, ingredient.name, recipe.matchedIngredients.some(x => x.id == ingredient.id));
        ingredientsWithIsMatchDto.push(ingredientWithIsMatchDto);
      });
      let recipeSearchResultDto = new RecipeSearchResultDto(recipe.id, recipe.title, recipe.host, recipe.url, recipe.imageUrl, ingredientsWithIsMatchDto, recipe.matchedIngredients.length);
      return recipeSearchResultDto;
    });
    this.setTitle();

  }
  setTitle() {
    this.title = this.isDetailedView ? 'Detaljer om opskriften' : this.recipes.length + ' opskrifter fundet'
  }
  setCurrentScrollTop(event) {
    this.currentScrollTop = event.detail.scrollTop;
    let content = this.getContent();
    if (this.isDetailedView == true) {
      if (this.currentScrollTop < this.lockedScrollTop) {
        console.log("scrolling above content");
      }
      else if (this.currentScrollTop > this.lockedScrollTop + this.currentSelectedRecipeContainerElement.clientHeight - content.clientHeight) {
        console.log("scrolling below content");
      }
    }
  }

  toggleDetails(e) {

    this.currentSelectedRecipeContainerElement = e.currentTarget;
    let clientYOffset = this.currentSelectedRecipeContainerElement.offsetTop;
    let clientXOffset = this.currentSelectedRecipeContainerElement.offsetLeft;
    let content = this.getContent();
    let allRecipeElements = document.querySelectorAll('.recipeContainer');
    allRecipeElements.forEach((recipeElement) => {
      if (recipeElement != this.currentSelectedRecipeContainerElement) {
        recipeElement.classList.toggle('hide');
      }
    })
    let selectedRecipeElement: HTMLElement = this.currentSelectedRecipeContainerElement.getElementsByClassName('recipe')[0] as HTMLElement;
    if (this.currentSelectedRecipeContainerElement.classList.contains("detailedView")) {
      this.isDetailedView = false;
      content.classList.remove('disableScroll');
      selectedRecipeElement.style.top = '0px';
      selectedRecipeElement.style.left = '0px';
      selectedRecipeElement.style.width = '100%';
    }
    else {
      this.isDetailedView = true;
      // selectedRecipeElement.style.top = (this.scrollTop - clientYOffset) + 'px';
      // selectedRecipeElement.style.left = '-' + clientXOffset + 'px';      
      selectedRecipeElement.style.top = (this.currentScrollTop - clientYOffset) + 'px';
      selectedRecipeElement.style.left = '-' + clientXOffset + 'px';
      selectedRecipeElement.style.width = '100vw';
    }
    this.lockedScrollTop = this.currentScrollTop;

    this.setTitle();
    this.currentSelectedRecipeContainerElement.classList.toggle('detailedView');
  }
  getContent() {
    return document.querySelector('ion-content');
  }
  goToUrl(event) {
    window.open(event, '_system')
  }
}
