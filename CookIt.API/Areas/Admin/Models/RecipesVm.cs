using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Areas.Admin.Models
{
    public class RecipesVm
    {
        public List<Recipe> Recipes { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public RecipesVm(List<Recipe> recipes, List<Ingredient> ingredients)
        {
            Recipes = recipes;
            Ingredients = ingredients;
        }
    }
}
