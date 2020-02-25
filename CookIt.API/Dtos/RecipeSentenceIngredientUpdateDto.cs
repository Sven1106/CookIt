using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public class RecipeSentenceIngredientUpdateDto
    {
        public Guid RecipeSentenceIngredientId { get; set; }
        public string IngredientIdOrNewIngredientName { get; set; }
    }
}
