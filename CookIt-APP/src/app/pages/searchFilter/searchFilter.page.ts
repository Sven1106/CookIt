import { Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Ingredient } from 'src/app/_models/ingredient';
import { Observable } from 'rxjs';
import { RecipeService } from 'src/app/_services/recipe/recipe.service';
import { AlertService } from 'src/app/_services/alert/alert.service';
import { HttpErrorResponse } from '@angular/common/http';
import { MatAutocompleteSelectedEvent } from '@angular/material';
import { IngredientWithIsDisabledDto } from 'src/app/_models/ingredientWithIsDisabledDto';
import { RecipeWithMatchedIngredients } from 'src/app/_models/recipeWithMatchedIngredients';
import { List } from 'lodash';


@Component({
  selector: 'app-searchFilter',
  templateUrl: './searchFilter.page.html',
  styleUrls: ['./searchFilter.page.scss']
})
export class SearchFilterPage implements OnInit {

  gettingRecipes: boolean;
  ingredientSearchLimit: number = 20;
  ingredientSearchForm = new FormControl();
  ingredientOptions: Ingredient[] = [];
  filteredIngredientList: Observable<Ingredient[]>;
  additionalIngredients: Ingredient[] = [];
  ingredientsInKitchenCupboard: IngredientWithIsDisabledDto[] = [];
  ingredientsForSubmit: Ingredient[] = [];
  constructor(
    private recipeService: RecipeService,
    private alertService: AlertService,
    private router: Router
  ) { }

  ngOnInit() {
    this.recipeService.ingredients.subscribe(ingredients => {
      this.ingredientOptions = ingredients.filter(x => x.id !== '00000000-0000-0000-0000-000000000000');
    });
    this.recipeService.ingredientsInKitchenCupboard.subscribe(ingredientsInKitchenCupboard => {
      this.ingredientsInKitchenCupboard = ingredientsInKitchenCupboard;
      this.updateFilteredIngredientOptions();
      this.prepareIngredientsForSubmit();
    });

  }
  ionViewWillEnter() {
    this.gettingRecipes = false;
    this.additionalIngredients = [];
    // this.recipeService.initData().subscribe(
    //   {
    //     next: () => {
    //       if (this.recipeService.ingredientsInKitchenCupboard === null) {
    //         this.ingredientsInKitchenCupboard = [];
    //       }
    //       else {
    //         this.ingredientsInKitchenCupboard = this.recipeService.orderByNameAsc(this.recipeService.ingredientsInKitchenCupboard.map(x => new IngredientWithIsDisabledDto(x.id, x.name)));
    //       }
    //       this.prepareData();
    //     },
    //     error: (error: any) => {
    //       console.error(error);
    //       if (error instanceof HttpErrorResponse) {
    //         switch (error.status) {
    //           case 400:
    //             console.log('Ingen opskrifter fundet eller ingen ændringer lavet');
    //             break;
    //           case 0:
    //             console.log('Ingen forbindelse til serveren');
    //             break;
    //           default:
    //             const applicationError = error.headers.get('Application-Error');
    //             if (applicationError) {
    //               console.log(applicationError);
    //             }
    //             else {
    //               console.log('Der opstod en fejl');
    //             }
    //             break;
    //         }
    //       }
    //     },
    //     complete: () => {
    //       console.log('complete');
    //     }
    //   });

  }

  private updateFilteredIngredientOptions() {
    const ingredientInRecipeSearchDtoToIngredients = this.ingredientsInKitchenCupboard.map(x => new Ingredient(x.id, x.name));
    const alreadySelectedIngredients = this.additionalIngredients.concat(ingredientInRecipeSearchDtoToIngredients);
    this.filteredIngredientList = this.recipeService.setIngredientSearchObservable(this.ingredientSearchForm, this.ingredientOptions, alreadySelectedIngredients);
  }

  displayIngredientName(ingredient: Ingredient): string {
    return ingredient && ingredient.name ? ingredient.name : '';
  }

  addIngredientToAdditionalIngredients(event: MatAutocompleteSelectedEvent) {
    const ingredient: Ingredient = event.option.value;
    this.additionalIngredients.push(ingredient);
    this.ingredientSearchForm.setValue('');
    this.updateFilteredIngredientOptions();
    this.prepareIngredientsForSubmit();
  }

  removeIngredientFromAdditionalIngredients(ingredient: Ingredient): void {
    const index = this.additionalIngredients.indexOf(ingredient);
    if (index >= 0) {
      this.additionalIngredients.splice(index, 1);
      this.updateFilteredIngredientOptions();
      this.prepareIngredientsForSubmit();
    }
  }
  toggleIngredientInKitchenCupboard(ingredient: IngredientWithIsDisabledDto): void {
    ingredient.isDisabled = !ingredient.isDisabled;
    this.prepareIngredientsForSubmit();
  }
  enableAllIngredientsInKitchenCupboard() {
    this.ingredientsInKitchenCupboard.forEach(element => {
      element.id = element.id;
      element.name = element.name;
      element.isDisabled = false;
    });
    this.ingredientsInKitchenCupboard = this.recipeService.orderByNameAsc(this.ingredientsInKitchenCupboard);
    this.prepareIngredientsForSubmit();
  }
  disableAllIngredientsInKitchenCupboard() {
    this.ingredientsInKitchenCupboard.forEach(element => {
      element.id = element.id;
      element.name = element.name;
      element.isDisabled = true;
    });
    this.ingredientsInKitchenCupboard = this.recipeService.orderByNameAsc(this.ingredientsInKitchenCupboard);
    this.prepareIngredientsForSubmit();
  }


  private prepareIngredientsForSubmit() {
    const activeIngredientsInKitchenCupboard = this.ingredientsInKitchenCupboard.filter(x => x.isDisabled === false);
    const activeIngredientsInKitchenCupboardCorrectlyFormatted = activeIngredientsInKitchenCupboard.map(x => new Ingredient(x.id, x.name));
    this.ingredientsForSubmit = this.additionalIngredients.concat(activeIngredientsInKitchenCupboardCorrectlyFormatted);
  }


  searchRecipes() {
    this.gettingRecipes = true;
    this.recipeService.searchForRecipes(this.ingredientsForSubmit).subscribe({
      next: (next: boolean) => {
        this.gettingRecipes = false;
        if (next === false) {
          this.alertService.success('Ingen opskrifter fundet', 2000);
        }
        else {
          this.router.navigateByUrl('tabs/recipes/searchResult');
        }
      },
      error: (error) => {
        if (error instanceof HttpErrorResponse) {
          switch (error.status) {
            case 400:
              this.alertService.error('Igen ingredienser matchede');
              break;
            case 0:
              this.alertService.error('Ingen forbindelse til serveren');
              break;
            default:
              const applicationError = error.headers.get('Application-Error');
              if (applicationError) {
                this.alertService.error(applicationError);
              }
              else {
                this.alertService.error('Der opstod en fejl');
              }
              break;
          }
          this.gettingRecipes = false;
        }
      }
    });
  }
}
