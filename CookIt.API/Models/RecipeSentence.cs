using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class RecipeSentence
    {
        public Guid Id { get; set; }
        public Recipe Recipe { get; set; }
        public string DerivedFrom { get; set; }
        public List<RecipeSentenceIngredient> RecipeSentenceIngredients { get; set; }
        public RecipeSentence()
        {

        }
        public RecipeSentence(Recipe Recipe, string DerivedFrom)
        {
            this.Id = Guid.NewGuid();
            this.Recipe = Recipe;
            this.DerivedFrom = DerivedFrom;
        }
    }
}
