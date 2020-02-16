using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public class RecipeForListDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Host Host { get; set; }
        public Uri Url { get; set; }
        public Uri ImageUrl { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public List<Ingredient> MatchedIngredients { get; set; }
    }

}
