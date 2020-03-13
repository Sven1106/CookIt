using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public class UpdateUserIngredientDto
    {
        public List<Ingredient> Ingredients { get; set; }
    }
}
