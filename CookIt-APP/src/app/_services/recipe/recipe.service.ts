import { Injectable } from '@angular/core';
import { HttpClient, HttpParams, HttpErrorResponse } from '@angular/common/http';
import { Observable, of, from, BehaviorSubject } from 'rxjs';
import { FormControl } from '@angular/forms';
import { map, startWith, switchMap, concatMap, mergeMap, flatMap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Ingredient } from 'src/app/_models/ingredient';
import { RecipeForListDto } from 'src/app/_models/recipeForListDto';
import { RecipeWithMatchedIngredients } from 'src/app/_models/recipeWithMatchedIngredients';
import { IngredientWithIsDisabledDto } from '../../_models/ingredientWithIsDisabledDto';
@Injectable({
  providedIn: 'root'
})
export class RecipeService {
  private ingredientsBehavior: BehaviorSubject<Ingredient[]> = new BehaviorSubject([]);
  private ingredientsInKitchenCupboardBehavior: BehaviorSubject<IngredientWithIsDisabledDto[]> = new BehaviorSubject([]);
  private searchResultRecipesBehavior: BehaviorSubject<RecipeWithMatchedIngredients[]> = new BehaviorSubject([]);
  private favoriteRecipesBehavior: BehaviorSubject<RecipeWithMatchedIngredients[]> = new BehaviorSubject([]);
  baseUrl = environment.apiDomain + 'api/';
  constructor(
    private httpClient: HttpClient
  ) {
    
  }

  initData() {
    this.getIngredients().subscribe(ingredientResult => {
      this.ingredientsBehavior.next(ingredientResult);
    },
      err => console.log("Error retrieving ingredients"));

    this.getUserIngredients().subscribe(userIngredientsResult => {
      let ingredientWithIsDisabled: IngredientWithIsDisabledDto[] = [];
      if (userIngredientsResult != null) {
        ingredientWithIsDisabled = this.orderByNameAsc(userIngredientsResult.map(x => new IngredientWithIsDisabledDto(x.id, x.name)));
      }
      this.ingredientsInKitchenCupboardBehavior.next(ingredientWithIsDisabled);

      this.getFavoriteRecipes(userIngredientsResult).subscribe(favoriteRecipeResult => {
        console.log(favoriteRecipeResult);
        let favoriteRecipes: RecipeWithMatchedIngredients[] = [];
        if (favoriteRecipeResult != null) {
          favoriteRecipes = favoriteRecipeResult;
        }
        this.favoriteRecipesBehavior.next(favoriteRecipes);
      });
    },
      err => console.log("Error retrieving kitchenCupboard"));
  }



  get ingredients() {
    return this.ingredientsBehavior.asObservable();
  }

  get ingredientsInKitchenCupboard() {
    return this.ingredientsInKitchenCupboardBehavior.asObservable();
  }
  get searchRecipes() {
    return this.searchResultRecipesBehavior.asObservable();
  }

  get favoriteRecipes() {
    return this.favoriteRecipesBehavior.asObservable();
  }


  searchForRecipes(ingredients: Ingredient[]): Observable<boolean> {
    return this.getRecipes(ingredients).pipe(map((recipesResult) => {
      let recipeWithMatchedIngredients: RecipeWithMatchedIngredients[] = [];
      if (recipesResult != null) {
        recipeWithMatchedIngredients = recipesResult;
      }
      this.searchResultRecipesBehavior.next(recipeWithMatchedIngredients);
      return recipeWithMatchedIngredients.length > 0;
    }));
  }

  getRecipes(ingredients: Ingredient[]): Observable<RecipeWithMatchedIngredients[]> {
    let params = new HttpParams();
    const missingIngredientsLimit = '10';
    ingredients.forEach(ingredient => {
      params = params.append('ingredientsIds', ingredient.id);
    });
    params = params.append('missingIngredientsLimit', missingIngredientsLimit);
    return this.httpClient.get<RecipeWithMatchedIngredients[]>(this.baseUrl + 'recipes/getRecipes', { params: params });
  }

  getFavoriteRecipes(ingredients: Ingredient[]): Observable<RecipeWithMatchedIngredients[]> {
    let params = new HttpParams();
    if (ingredients == null) {
      ingredients = [];
    }
    ingredients.forEach(ingredient => {
      params = params.append('ingredientsIds', ingredient.id);
    });
    return this.httpClient.get<RecipeWithMatchedIngredients[]>(this.baseUrl + 'user/getFavoriteRecipes', { params: params });
  }

  toggleFavoriteRecipe(recipe: RecipeForListDto) {
    const obs: Observable<any> = this.httpClient.post<RecipeWithMatchedIngredients>(this.baseUrl + 'user/toggleFavoriteRecipe/' + recipe.id, '');
    obs.subscribe({
      next: () => {
        let favoriteRecipes = this.favoriteRecipesBehavior.getValue();
        let currentSearches = this.searchResultRecipesBehavior.getValue();
        if (favoriteRecipes === null) {
          favoriteRecipes = [];
        }
        if (currentSearches === null) {
          currentSearches = [];
        }
        if (recipe.isFavorite) {
          const userIngredients = this.ingredientsInKitchenCupboardBehavior.getValue();
          this.getFavoriteRecipes(userIngredients).subscribe((newUserIngredients) => {
            const newRecipes = newUserIngredients.filter(x => favoriteRecipes.indexOf(x) === -1);
            this.favoriteRecipesBehavior.next(newRecipes);

            newRecipes.forEach(newRecipe => {
              const indexOfNewRecipe = currentSearches.findIndex((recipeFromSearch: RecipeWithMatchedIngredients) => recipeFromSearch.id === newRecipe.id);
              console.log(indexOfNewRecipe);
              if (indexOfNewRecipe !== -1) {
                console.log('doStuffToSearches');
              }
            });

            // this.searchResultRecipesBehavior.next()
          });
        }
        else {
          const favoriteRecipeIndex = favoriteRecipes.findIndex((favoriteRecipe: RecipeWithMatchedIngredients) => favoriteRecipe.id === recipe.id);
          favoriteRecipes.splice(favoriteRecipeIndex, 1);
          this.favoriteRecipesBehavior.next(favoriteRecipes);



          // newRecipes.forEach(newRecipe => {
          //   const indexOfNewRecipe = currentSearches.indexOf(newRecipe);
          //   console.log(indexOfNewRecipe);
          //   if (indexOfNewRecipe !== -1) {
          //     console.log('doStuffToSearches');
          //   }
          // });
        }
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
        // console.log("complete");
      }
    });
  }



  getIngredients(): Observable<Ingredient[]> {
    return this.httpClient.get<Ingredient[]>(this.baseUrl + 'recipes/getIngredients');
  }


  getUserIngredients(): Observable<Ingredient[]> {
    return this.httpClient.get<Ingredient[]>(this.baseUrl + 'user/getUserIngredients');
  }
  updateUserIngredients(ingredients: Ingredient[]): Observable<any> {
    if (ingredients === null) {
      ingredients = [];
    }
    const updateUserIngredientDto = {
      Ingredients: ingredients
    };
    let obs: Observable<any> = this.httpClient.post(this.baseUrl + 'user/updateUserIngredients', updateUserIngredientDto);
    obs.subscribe(() => {
      this.ingredientsInKitchenCupboardBehavior.next(this.orderByNameAsc(ingredients.map(x => new IngredientWithIsDisabledDto(x.id, x.name))));

      const userIngredients = this.ingredientsInKitchenCupboardBehavior.getValue();
      this.getFavoriteRecipes(userIngredients).subscribe(favoriteRecipeResult => {
        console.log(favoriteRecipeResult);
        let favoriteRecipes: RecipeWithMatchedIngredients[] = [];
        if (favoriteRecipeResult != null) {
          favoriteRecipes = favoriteRecipeResult;
        }
        this.favoriteRecipesBehavior.next(favoriteRecipes);
      });
    });
    return obs;
  }
  orderByNameAsc(array: any): any {
    return array.sort((a, b) => {
      if (a.name < b.name) { return -1; }
      if (a.name > b.name) { return 1; }
      return 0;
    });
  }
  //#region Move to Component
  setIngredientSearchObservable(ingredientSearchForm: FormControl, unfilteredIngredients: Ingredient[], ingredientsToIgnore: Ingredient[]): Observable<any> {
    return ingredientSearchForm.valueChanges
      .pipe(
        startWith(''),
        map(filterValue => typeof filterValue === 'string' ? filterValue : filterValue.name),
        map(filterValue => this.filterIngredients(filterValue, unfilteredIngredients, ingredientsToIgnore))
      );
  }
  private filterIngredients(value: string = '', unfilteredIngredients: Ingredient[], ingredientsToIgnore: Ingredient[]): Ingredient[] {
    const filterValue = value.toLowerCase();
    let ingredientsWithOutMyIngredients: Ingredient[] = [];
    if (filterValue !== '') {
      // TODO Implement this: let startingWithFilterValue = unfilteredIngredientList.filter(ingredient => ingredient.name.toLowerCase().indexOf(filterValue) === 0);  to filter ingredients that starts with filterValue
      const containsFilterValue = unfilteredIngredients.filter(ingredient => ingredient.name.toLowerCase().indexOf(filterValue.toLowerCase()) > -1);
      const sortedAfterShortestEditDistance = this.levenshteinIngredientArray(filterValue, containsFilterValue);
      ingredientsWithOutMyIngredients = sortedAfterShortestEditDistance.filter((objFromA) => !ingredientsToIgnore.find((objFromB) => objFromA.id === objFromB.id));
    }
    return ingredientsWithOutMyIngredients;
  }
  private levenshteinIngredientArray = (searchValue: string, itemArray: Ingredient[]) => {
    const arrayWithDistance = itemArray.map((ingredient: Ingredient) => {
      return {
        item: ingredient,
        distance: this.levenshteinDistance(searchValue, ingredient.name)
      };
    });
    const sortedArray = arrayWithDistance.sort((a, b) => a.distance - b.distance);
    return sortedArray.map(x => x.item);
  };
  private levenshteinDistance = (r: string, a: string) => {
    let t = [], f = r.length, n = a.length; if (0 == f) return n; if (0 == n) return f; for (let v = f; v >= 0; v--)t[v] = []; for (let v = f; v >= 0; v--)t[v][0] = v; for (let e = n; e >= 0; e--)t[0][e] = e; for (let v = 1; f >= v; v++)for (let h = r.charAt(v - 1), e = 1; n >= e; e++) { if (v == e && t[v][e] > 4) return f; let i = a.charAt(e - 1), o = h == i ? 0 : 1, c = t[v - 1][e] + 1, u = t[v][e - 1] + 1, A = t[v - 1][e - 1] + o; c > u && (c = u), c > A && (c = A), t[v][e] = c, v > 1 && e > 1 && h == a.charAt(e - 2) && r.charAt(v - 2) == i && (t[v][e] = Math.min(t[v][e], t[v - 2][e - 2] + o)) } return t[f][n]
  };
  //#endregion
}
