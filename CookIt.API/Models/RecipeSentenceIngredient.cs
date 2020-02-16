using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class RecipeSentenceIngredient
    {
        public Guid Id { get; set; }
        public RecipeSentence RecipeSentence { get; set; }
        public Ingredient Ingredient { get; set; }
        public RecipeSentenceIngredient()
        {

        }
        public RecipeSentenceIngredient(RecipeSentence recipeSentence, Ingredient ingredient)
        {
            this.Id = Guid.NewGuid();
            RecipeSentence = recipeSentence;
            Ingredient = ingredient;
        }
    }
}
