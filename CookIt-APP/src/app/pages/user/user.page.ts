import { AlertService } from './../../_services/alert.service';
import { Ingredient } from './../../_models/ingredient';
import { RecipeService } from './../../_services/recipe.service';
import { Component, OnInit } from '@angular/core';
import { map, startWith } from 'rxjs/operators';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { MatAutocompleteSelectedEvent } from '@angular/material';

@Component({
  selector: 'app-user',
  templateUrl: './user.page.html',
  styleUrls: ['./user.page.scss'],
})
export class UserPage implements OnInit {

  selectedSegment: string;
  ingredientSearchLimit: number = 20;
  ingredientSearchForm = new FormControl();
  ingredientOptions: Ingredient[] = [];
  filteredIngredientOptions: Observable<Ingredient[]>;
  myIngredients: Ingredient[] = [];
  constructor(
    private recipeService: RecipeService,
    private alertService: AlertService
  ) {
   
  }
  ngOnInit() {
    this.filteredIngredientOptions = this.ingredientSearchForm.valueChanges
      .pipe(
        startWith(''),
        map(value => typeof value === 'string' ? value : value.name),
        map(name => name ? this.filterIngredients(name) : this.ingredientOptions.slice().filter((objFromA) => !this.myIngredients.find((objFromB) => objFromA.id === objFromB.id)))
      );

    this.loadData();
    this.recipeService.getMyIngredientsInStorage().then(ingredients => {
      if (ingredients == null) {
        this.myIngredients = [];
      }
      else {
        this.myIngredients = ingredients;
      }
    });
  }
  ionViewWillEnter() {
    this.selectedSegment = "myIngredients";
  }

  displayFn(ingredient: Ingredient): string {
    return ingredient && ingredient.name ? ingredient.name : '';
  }
  private filterIngredients(value: string): Ingredient[] {
    const filterValue = value.toLowerCase();

    //TODO Implement this: let startingWithFilterValue = this.ingredientOptions.filter(ingredient => ingredient.name.toLowerCase().indexOf(filterValue) === 0);  to filter ingredients that starts with filterValue
    let containsFilterValue = this.ingredientOptions.filter(ingredient => ingredient.name.toLowerCase().indexOf(filterValue.toLowerCase()) > -1);
    let sortedAfterShortestEditDistance = this.levenshteinIngredientArray(filterValue, containsFilterValue);
    let ingredientsWithOutMyIngredients = sortedAfterShortestEditDistance.filter((objFromA) => !this.myIngredients.find((objFromB) => objFromA.id === objFromB.id));
    return ingredientsWithOutMyIngredients
  }

  loadData() {
    this.recipeService.getIngredients().subscribe({
      next: (next) => {
        this.ingredientOptions = next.filter(x => x.id != '00000000-0000-0000-0000-000000000000');
      },
      error: (error) => {
        console.log("loadData - Error");
        console.log(error);
      }
    });
  }
  levenshteinIngredientArray = (searchValue: string, itemArray: Ingredient[]) => {
    let arrayWithDistance = itemArray.map((ingredient: Ingredient) => {
      return {
        item: ingredient,
        distance: this.levenshteinDistance(searchValue, ingredient.name)
      };
    });
    let sortedArray = arrayWithDistance.sort((a, b) => a.distance - b.distance);
    return sortedArray.map(x => x.item);
  };
  levenshteinDistance = (r: string, a: string) => {
    let t = [], f = r.length, n = a.length; if (0 == f) return n; if (0 == n) return f; for (let v = f; v >= 0; v--)t[v] = []; for (let v = f; v >= 0; v--)t[v][0] = v; for (let e = n; e >= 0; e--)t[0][e] = e; for (let v = 1; f >= v; v++)for (let h = r.charAt(v - 1), e = 1; n >= e; e++) { if (v == e && t[v][e] > 4) return f; let i = a.charAt(e - 1), o = h == i ? 0 : 1, c = t[v - 1][e] + 1, u = t[v][e - 1] + 1, A = t[v - 1][e - 1] + o; c > u && (c = u), c > A && (c = A), t[v][e] = c, v > 1 && e > 1 && h == a.charAt(e - 2) && r.charAt(v - 2) == i && (t[v][e] = Math.min(t[v][e], t[v - 2][e - 2] + o)) } return t[f][n]
  };

  saveChanges() {
    this.recipeService.setMyIngredientsInStorage(this.myIngredients).then(() => {
      this.alertService.success("Ændringerne blev gemt");
    });
  }
  addMyIngredient(event: MatAutocompleteSelectedEvent) {
    let ingredient: Ingredient = event.option.value;
    this.myIngredients.push(ingredient);
    this.ingredientSearchForm.setValue('');
  }

  removeMyIngredient(ingredient: Ingredient): void {
    const index = this.myIngredients.indexOf(ingredient);
    if (index >= 0) {
      this.myIngredients.splice(index, 1);
    }
  }
}
