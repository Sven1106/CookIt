using CookIt.API.Core;
using CookIt.API.Dtos;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Interfaces
{
    public interface IRecipeRepository
    {
        int CreateRecipes(CreateRecipeDto createRecipeDto);
        Recipe GetRecipe(Guid id);
        List<Recipe> GetRecipes();
        List<RecipeForListDto> GetFilteredRecipes(RecipeFilter filter);
        int DeleteRecipe(Guid id);
        RecipeSentenceIngredient GetRecipeSentenceIngredient(Guid id);
        int UpdateRecipeSentenceIngredient(Guid id, string ingredientValue);
        int DeleteRecipeSentenceIngredient(Guid id);
    }
}
