import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { MatAutocompleteSelectedEvent } from '@angular/material/autocomplete';
import { Observable } from 'rxjs';
import { AlertService } from '@core/services/alert/alert.service';
import { Ingredient } from '@core/models/ingredient';
import { RecipeService } from '@core/services/recipe/recipe.service';


@Component({
  selector: 'app-user',
  templateUrl: './user.page.html',
  styleUrls: ['./user.page.scss'],
})
export class UserPage implements OnInit {

  anyChanges: boolean;
  selectedSegment: string;
  ingredientSearchLimit: number = 20;
  ingredientSearchForm = new FormControl();
  ingredientOptions: Ingredient[] = [];
  filteredIngredientList: Observable<Ingredient[]>;
  ingredientsInKitchenCupboard: Ingredient[] = [];
  modifiedIngredientsInKitchenCupboard: Ingredient[] = [];
  anyIngredientsInKitchenCupboardChanges: boolean = false;
  constructor(
    private recipeService: RecipeService,
    private alertService: AlertService
  ) {
  }
  ngOnInit() {
    this.recipeService.ingredients.subscribe(ingredients => {
      this.ingredientOptions = ingredients.filter(x => x.id !== '00000000-0000-0000-0000-000000000000');
    });
    this.recipeService.ingredientsInKitchenCupboard.subscribe(ingredientsInKitchenCupboard => {
      this.ingredientsInKitchenCupboard = ingredientsInKitchenCupboard;
      this.modifiedIngredientsInKitchenCupboard = this.ingredientsInKitchenCupboard.map(x => x);
      this.filteredIngredientList = this.recipeService.setIngredientSearchObservable(this.ingredientSearchForm, this.ingredientOptions,  this.modifiedIngredientsInKitchenCupboard);
    });
  }
  ionViewWillLeave() {
    // ToDO
    // if (this.anyIngredientsInKitchenCupboardChanges) {
    //   console.log('unsaved changes');

    //   if (true) {
    //     this.anyIngredientsInKitchenCupboardChanges = false;
    //   }
    //   else {

    //   }
    // }
  }
  ionViewWillEnter() {
    this.selectedSegment = 'kitchenCupboard';
  }

  displayIngredientName(ingredient: Ingredient): string {
    return ingredient && ingredient.name ? ingredient.name : '';
  }

  saveChanges() {
    this.recipeService.updateUserIngredients(this.modifiedIngredientsInKitchenCupboard)
      .subscribe(
        {
          next: () => {
            this.alertService.success('Ændringerne blev gemt', 1500);
            this.anyIngredientsInKitchenCupboardChanges = false;
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
            console.log('complete');
          }
        });

  }
  addIngredientToKitchenCupboard(event: MatAutocompleteSelectedEvent) {
    let ingredient: Ingredient = event.option.value;
    this.modifiedIngredientsInKitchenCupboard.push(ingredient);
    this.ingredientSearchForm.setValue('');
    this.modifiedIngredientsInKitchenCupboard = this.recipeService.orderByNameAsc(this.modifiedIngredientsInKitchenCupboard);
    this.anyIngredientsInKitchenCupboardChanges = true;
  }

  removeIngredientFromKitchenCupboard(ingredient: Ingredient): void {
    this.anyChanges = true;
    const index = this.modifiedIngredientsInKitchenCupboard.indexOf(ingredient);
    if (index >= 0) {
      this.modifiedIngredientsInKitchenCupboard.splice(index, 1);
      this.modifiedIngredientsInKitchenCupboard = this.recipeService.orderByNameAsc(this.modifiedIngredientsInKitchenCupboard);
      this.anyIngredientsInKitchenCupboardChanges = true;
    }
  }
}
