using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using CookIt.API.Dtos;

namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;

        public UserController(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository)
        {
            _recipeRepository = recipeRepository;
            _ingredientRepository = ingredientRepository;
        }
        [HttpPost("toggleFavoriteRecipe/{recipeId}")]
        public async Task<IActionResult> ToggleFavoriteRecipe(Guid recipeId)
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            {
                Recipe recipe = await _recipeRepository.GetRecipeAsync(recipeId);
                if (recipe == null)
                {
                    return BadRequest("No recipe found");
                }

                int changesMade = await _recipeRepository.ToggleFavoriteRecipeAsync(userId, recipeId);
                if (changesMade == 0)
                {
                    return BadRequest("No changes were made");
                }
            }
            else
            {
                return StatusCode(500, "No valid userId found in claims");
            }
            return Ok();
        }

        [HttpGet("getFavoriteRecipes")]
        public async Task<IActionResult> GetFavoriteRecipes([FromQuery]GetFavoriteRecipesDto filter)
        {
            List<RecipeWithMatchedIngredientsDto> recipesWithMatchedIngredients = new List<RecipeWithMatchedIngredientsDto>();
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            {
                recipesWithMatchedIngredients = await _recipeRepository.GetFavoriteRecipesAsync(filter, userId);

                if (recipesWithMatchedIngredients == null)
                {
                    return StatusCode(500, "GetFavoriteRecipes failed");
                }
                if (recipesWithMatchedIngredients.Count == 0)
                {
                    return NoContent();
                }

            }
            else
            {
                return StatusCode(500, "No valid userId found in claims");
            }
            return Ok(recipesWithMatchedIngredients);
        }








        [HttpGet("getUserIngredients")]
        public async Task<IActionResult> GetUserIngredients()
        {
            List<Ingredient> userIngredients = new List<Ingredient>();
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            {
                userIngredients = await _ingredientRepository.GetUserIngredients(userId);

                if (userIngredients == null)
                {
                    return StatusCode(500, "GetAllUserIngredients failed");
                }

                if (userIngredients.Count == 0)
                {
                    return NoContent();
                }
            }
            else
            {
                return StatusCode(500, "No valid userId found in claims");
            }
            return Ok(userIngredients);
        }

        [HttpPost("updateUserIngredients")]
        public async Task<IActionResult> UpdateUserIngredients(UpdateUserIngredientDto updateUserIngredientDto)
        {
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            {
                int changesMade = await _ingredientRepository.UpdateUserIngredients(userId, updateUserIngredientDto.Ingredients);
                if (changesMade == 0)
                {
                    return BadRequest("No changes were made");
                }
            }
            else
            {
                return StatusCode(500, "No valid userId found in claims");
            }
            return Ok();
        }

    }
}