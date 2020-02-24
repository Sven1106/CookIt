using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Repositories
{
    public class IngredientRepository: IIngredientRepository
    {
        private readonly AppDbContext _appDbContext;
        public IngredientRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public List<Ingredient> GetIngredients()
        {
            List<Ingredient> ingredients = _appDbContext.Ingredient
               .AsNoTracking()
               .ToList();
            return ingredients;
        }
    }
}
