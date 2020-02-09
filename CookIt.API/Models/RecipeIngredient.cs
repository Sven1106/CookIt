using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class RecipeIngredient
    {
        public Guid Id { get; set; }
        public Recipe Recipe { get; set; }
        public Ingredient Ingredient { get; set; }
        public string DerivedFrom { get; set; }
        public RecipeIngredient()
        {

        }
        public RecipeIngredient(Recipe Recipe, Ingredient Ingredient, string DerivedFrom)
        {
            this.Id = Guid.NewGuid();
            this.Recipe = Recipe;
            this.Ingredient = Ingredient;
            this.DerivedFrom = DerivedFrom;
        }
    }
}
