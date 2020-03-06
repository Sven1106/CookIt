import { Ingredient } from './../_models/ingredient';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Storage } from '@ionic/storage';
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
  getRecipes() {
    this.http.get(this.baseUrl + 'recipes/getRecipes?IngredientsIds=10a83da2-7828-4e17-83a3-48e4e3c01670')
      .subscribe(response => {
        this.recipes = response;
        console.log(this.recipes);
      }, error => {
        console.log(error);
      });
  }
  getIngredients(): Observable<Ingredient[]> {
    return this.http.get<Ingredient[]>(this.baseUrl + 'recipes/getIngredients');
  }
  async getMyIngredientsInStorage() {
    return await this.storage.get('myIngredients').then((myIngredients: Ingredient[]) => {
      console.log("get: " + JSON.stringify(myIngredients));
      return myIngredients;
    });
  }

  async setMyIngredientsInStorage(myIngredients: Ingredient[]) {
    console.log("set: " + JSON.stringify(myIngredients));
    await this.storage.set('myIngredients', myIngredients);
  }

  removeMyIngredientsInStorage(): void {
    this.storage.remove('myIngredients');
  }
}
