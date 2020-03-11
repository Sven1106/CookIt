﻿using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public class FavoriteRecipeDto
    {
        public Guid Id { get; set; }
        public RecipeWithMatchedIngredientsDto Recipe { get; set; }

    }
}