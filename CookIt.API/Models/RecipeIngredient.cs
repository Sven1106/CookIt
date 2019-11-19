using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class RecipeIngredient
    {
        public Guid Id { get; set; }
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
    }
}
