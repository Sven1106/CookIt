using CookIt.API.Data;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Repositories
{
    public class IngredientRepository : IIngredientRepository
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

        public async Task<List<Ingredient>> GetUserIngredients(Guid userId)
        {
            User user = await _appDbContext.User.FindAsync(userId);
            List<Ingredient> ingredientsOnUser = new List<Ingredient>();
            try
            {
                ingredientsOnUser = JsonConvert.DeserializeObject<List<Ingredient>>(user.UserIngredients);
            }
            catch (Exception ex)
            {

            }
            return ingredientsOnUser;
        }

        public async Task<int> UpdateUserIngredients(Guid userId, List<Ingredient> ingredients)
        {
            User user = await _appDbContext.User.FindAsync(userId);
            if (user != null)
            {
                user.UserIngredients = JsonConvert.SerializeObject(ingredients);
            }
            return await _appDbContext.SaveChangesAsync();
        }
    }
}
