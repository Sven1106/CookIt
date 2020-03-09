import { AlertService } from 'src/app/_services/alert.service';
import { Ingredient } from 'src/app/_models/ingredient';
import { RecipeService } from 'src/app/_services/recipe.service';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material';
import { HttpErrorResponse } from '@angular/common/http';


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
  filteredIngredientOptions: Observable<Ingredient[]>;
  ingredientsInKitchenCupboard: Ingredient[] = [];
  anyIngredientsInKitchenCupboardChanges: boolean = false;
  constructor(
    private recipeService: RecipeService,
    private alertService: AlertService
  ) {
  }
  ngOnInit() {
    this.recipeService.getIngredientsFromKitchenCupboardInStorage().then(ingredients => {
      if (ingredients == null) {
        this.ingredientsInKitchenCupboard = [];
      }
      else {
        this.ingredientsInKitchenCupboard = this.recipeService.orderByNameAsc(ingredients);
      }
      this.prepareData();
    });
  }
  ionViewWillLeave() {
    if (this.anyIngredientsInKitchenCupboardChanges) {
      console.log("unsaved changes");

      if (true) {
        this.anyIngredientsInKitchenCupboardChanges = false;
      }
      else {

      }
    }
  }
  ionViewWillEnter() {
    this.selectedSegment = 'kitchenCupboard';
  }

  displayIngredientName(ingredient: Ingredient): string {
    return ingredient && ingredient.name ? ingredient.name : '';
  }
  prepareData() {
    this.recipeService.getIngredients().subscribe({
      next: (next: Ingredient[]) => {
        this.ingredientOptions = next.filter(x => x.id != '00000000-0000-0000-0000-000000000000');
        this.filteredIngredientOptions = this.recipeService.setIngredientSearchObservable(this.ingredientSearchForm, this.ingredientOptions, this.ingredientsInKitchenCupboard);
      },
      error: (error) => {
        if (error instanceof HttpErrorResponse) {
          switch (error.status) {
            case 400:
              this.alertService.error('Igen ingredienser fundet');
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
        }
      }
    });
  }

  saveChanges() {
    this.recipeService.setIngredientsInKitchenCupboardInStorage(this.ingredientsInKitchenCupboard).then(() => {
      this.alertService.success("Ændringerne blev gemt", 1500);
      this.anyIngredientsInKitchenCupboardChanges = false;
    });
  }
  addIngredientToKitchenCupboard(event: MatAutocompleteSelectedEvent) {
    let ingredient: Ingredient = event.option.value;
    this.ingredientsInKitchenCupboard.push(ingredient);
    this.ingredientSearchForm.setValue('');
    this.ingredientsInKitchenCupboard = this.recipeService.orderByNameAsc(this.ingredientsInKitchenCupboard);
    this.anyIngredientsInKitchenCupboardChanges = true;
  }

  removeIngredientFromKitchenCupboard(ingredient: Ingredient): void {
    this.anyChanges = true;
    const index = this.ingredientsInKitchenCupboard.indexOf(ingredient);
    if (index >= 0) {
      this.ingredientsInKitchenCupboard.splice(index, 1);
      this.ingredientsInKitchenCupboard = this.recipeService.orderByNameAsc(this.ingredientsInKitchenCupboard);
      this.anyIngredientsInKitchenCupboardChanges = true;
    }
  }
}
