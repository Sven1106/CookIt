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
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
        public string DerivedFrom { get; set; }
        public RecipeIngredient()
        {

        }
        public RecipeIngredient(Guid RecipeId, Guid IngredientId, string DerivedFrom)
        {
            this.Id = Guid.NewGuid();
            this.RecipeId = RecipeId;
            this.IngredientId = IngredientId;
            this.DerivedFrom = DerivedFrom;
        }
    }
}
