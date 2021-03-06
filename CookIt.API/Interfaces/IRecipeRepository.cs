﻿using CookIt.API.Dtos;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Interfaces
{
    public interface IRecipeRepository
    {
        Task<int> CreateRecipesAsync(CreateRecipeDto createRecipeDto);
        Task<Recipe> GetRecipeAsync(Guid id);
        Task<List<Recipe>> GetRecipesAsync();
        Task<List<RecipeWithMatchedIngredientsDto>> GetFilteredRecipesAsync(GetRecipesFilterDto filter, Guid id);
        Task<List<RecipeWithMatchedIngredientsDto>> GetFavoriteRecipesAsync(GetFavoriteRecipesDto filter, Guid id);
        Task<int> DeleteRecipeAsync(Guid id);
        Task<RecipeSentenceIngredient> GetRecipeSentenceIngredientAsync(Guid id);
        Task<int> UpdateRecipeSentenceIngredientAsync(Guid id, string ingredientValue);
        Task<int> DeleteRecipeSentenceIngredientAsync(Guid id);
        Task<int> ToggleFavoriteRecipeAsync(Guid userId, Guid recipe);
    }
}
