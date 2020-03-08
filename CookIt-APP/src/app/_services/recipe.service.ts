import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Storage } from '@ionic/storage';
import { FormControl } from '@angular/forms';
import { map, startWith } from 'rxjs/operators';
import { Ingredient } from 'src/app/_models/ingredient';
import { Recipe } from 'src/app/_models/recipe';
@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  baseUrl = environment.apiDomain + 'api/';
  recipes: any;
  constructor(
    private http: HttpClient,
    private storage: Storage
  ) {


  }

  ngOnInit() {

  }
  getRecipes(ingredients: Ingredient[]): Observable<Recipe[]> {
    let params = new HttpParams();
    ingredients.forEach(ingredient => {
      params = params.append('ingredientsIds', ingredient.id);
    });

    return this.http.get<Recipe[]>(this.baseUrl + 'recipes/getRecipes', { params: params });
  }
  getIngredients(): Observable<Ingredient[]> {
    return this.http.get<Ingredient[]>(this.baseUrl + 'recipes/getIngredients');
  }
  async getIngredientsFromKitchenCupboardInStorage() {
    return await this.storage.get('kitchenCupboard').then((kitchenCupboard: Ingredient[]) => {
      //console.log("get: " + JSON.stringify(kitchenCupboard));
      return kitchenCupboard;
    });
  }

  async setIngredientsInKitchenCupboardInStorage(ingredients: Ingredient[]) {
    //console.log("set: " + JSON.stringify(ingredients));
    await this.storage.set('kitchenCupboard', ingredients);
  }

  removeKitchenCupboardInStorage(): void {
    this.storage.remove('kitchenCupboard');
  }

  orderAsc(array: any, propertyname: string): any {
    return array.sort(function (a, b) {
      if (eval("a." + propertyname) < eval("b." + propertyname)) { return -1; }
      if (eval("a." + propertyname) > eval("b." + propertyname)) { return 1; }
      return 0;
    })
  }
  //#region Move to Component
  setIngredientSearchObservable(ingredientSearchForm: FormControl, unfilteredIngredientList: Ingredient[], ingredientsToIgnore: Ingredient[]): Observable<any> {
    return ingredientSearchForm.valueChanges
      .pipe(
        startWith(''),
        map(filterValue => typeof filterValue === 'string' ? filterValue : filterValue.name),
        map(filterValue => this.filterIngredients(filterValue, unfilteredIngredientList, ingredientsToIgnore))
      );
  }
  private filterIngredients(value: string = '', unfilteredIngredientList: Ingredient[], ingredientsToIgnore: Ingredient[]): Ingredient[] {
    const filterValue = value.toLowerCase();
    let ingredientsWithOutMyIngredients: Ingredient[] = [];
    if (filterValue != '') {
      //TODO Implement this: let startingWithFilterValue = unfilteredIngredientList.filter(ingredient => ingredient.name.toLowerCase().indexOf(filterValue) === 0);  to filter ingredients that starts with filterValue
      let containsFilterValue = unfilteredIngredientList.filter(ingredient => ingredient.name.toLowerCase().indexOf(filterValue.toLowerCase()) > -1);
      let sortedAfterShortestEditDistance = this.levenshteinIngredientArray(filterValue, containsFilterValue);
      ingredientsWithOutMyIngredients = sortedAfterShortestEditDistance.filter((objFromA) => !ingredientsToIgnore.find((objFromB) => objFromA.id === objFromB.id));
    }
    return ingredientsWithOutMyIngredients
  }
  private levenshteinIngredientArray = (searchValue: string, itemArray: Ingredient[]) => {
    let arrayWithDistance = itemArray.map((ingredient: Ingredient) => {
      return {
        item: ingredient,
        distance: this.levenshteinDistance(searchValue, ingredient.name)
      };
    });
    let sortedArray = arrayWithDistance.sort((a, b) => a.distance - b.distance);
    return sortedArray.map(x => x.item);
  };
  private levenshteinDistance = (r: string, a: string) => {
    let t = [], f = r.length, n = a.length; if (0 == f) return n; if (0 == n) return f; for (let v = f; v >= 0; v--)t[v] = []; for (let v = f; v >= 0; v--)t[v][0] = v; for (let e = n; e >= 0; e--)t[0][e] = e; for (let v = 1; f >= v; v++)for (let h = r.charAt(v - 1), e = 1; n >= e; e++) { if (v == e && t[v][e] > 4) return f; let i = a.charAt(e - 1), o = h == i ? 0 : 1, c = t[v - 1][e] + 1, u = t[v][e - 1] + 1, A = t[v - 1][e - 1] + o; c > u && (c = u), c > A && (c = A), t[v][e] = c, v > 1 && e > 1 && h == a.charAt(e - 2) && r.charAt(v - 2) == i && (t[v][e] = Math.min(t[v][e], t[v - 2][e - 2] + o)) } return t[f][n]
  };
  //#endregion

}
