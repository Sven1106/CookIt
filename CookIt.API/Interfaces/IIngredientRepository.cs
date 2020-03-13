using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Interfaces
{
    public interface IIngredientRepository
    {
        Task<List<Ingredient>> GetIngredients();
        Task<List<Ingredient>> GetUserIngredients(Guid userId);
        Task<int> UpdateUserIngredients(Guid userId, List<Ingredient> ingredients);
    }
}
