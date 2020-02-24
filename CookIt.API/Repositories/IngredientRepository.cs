using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<List<Ingredient>> GetIngredients()
        {
            List<Ingredient> ingredients = await _appDbContext.Ingredient
               .AsNoTracking()
               .ToListAsync();
            return ingredients;
        }
    }
}
