import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { AuthService } from '../_services/auth.service';

@Component({
  selector: 'app-recipe',
  templateUrl: './recipe.component.html',
  styleUrls: ['./recipe.component.scss'],
})
export class RecipeComponent implements OnInit {
  baseUrl = environment.apiDomain + 'api/';
  recipes: any;
  ingredients: any;
  constructor( private http: HttpClient, private auth: AuthService ) {


  }

  ngOnInit() {
    this.getRecipes();
    this.getIngredients();
    // this.auth.register({"name":"user","email":"user@user.dk","password":"user"}).subscribe(response => {
    //   console.log("Successful");
    // },
    // error => {
    //   console.log(error);
    // });
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
  getIngredients(){
    this.http.get(this.baseUrl + 'recipes/getIngredients')
      .subscribe(response => {
        this.ingredients = response;
        console.log(this.ingredients);
      }, error => {
        console.log(error);
      });
  }


  
}
