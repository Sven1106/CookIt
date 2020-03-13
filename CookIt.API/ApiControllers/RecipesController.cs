using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CookIt.API.Dtos;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Recipe = CookIt.API.Models.Recipe;

namespace CookIt.API.Controllers
{
    [ApiController]
    /*
        This is a mixed Authorized Controller. It can be authorized with cookies or JWT.
        The Ajax requests made to this controller are from same origin which means the cookieauthorization is used.
    */
    [Authorize(AuthenticationSchemes = AuthSchemes, Policy = "", Roles = Role.Admin)]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme; // Authorizes against both Schemes.
        private readonly IRecipeRepository _recipeRepository;
        private readonly IIngredientRepository _ingredientRepository;

        public RecipesController(IRecipeRepository recipeRepository, IIngredientRepository ingredientRepository)
        {
            _recipeRepository = recipeRepository;
            _ingredientRepository = ingredientRepository;
        }

        [HttpGet("getIngredients"), AllowAnonymous]
        public async Task<ActionResult> GetIngredientsAsync()
        {
            List<Ingredient> ingredients = await _ingredientRepository.GetIngredients();
            if (ingredients == null)
            {
                return StatusCode(500, "GetIngredients failed");
            }
            if (ingredients.Count == 0)
            {
                return BadRequest("No ingredients found");
            }
            return Ok(ingredients);
        }
        [HttpGet("getRecipes")]
        public async Task<ActionResult> GetRecipesAsync([FromQuery]GetRecipesFilterDto filter)
        {
            List<RecipeWithMatchedIngredientsDto> recipesWithMatchedIngredients = new List<RecipeWithMatchedIngredientsDto>();
            if (Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value, out Guid userId))
            {
                if (filter.IngredientsIds == null || filter.IngredientsIds.Count == 0)
                {
                    return BadRequest("Invalid Dto");
                }

                recipesWithMatchedIngredients = await _recipeRepository.GetFilteredRecipesAsync(filter, userId);
                if (recipesWithMatchedIngredients == null)
                {
                    return StatusCode(500, "GetRecipes failed");
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
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRecipeAsync(Guid id)
        {
            Recipe recipe = await _recipeRepository.GetRecipeAsync(id);
            if (recipe == null)
            {
                return BadRequest("No recipe found");
            }
            return Ok(recipe);
        }

        [HttpPost("updateRecipeSentenceIngredient")]
        public async Task<IActionResult> UpdateRecipeSentenceIngredientAsync(RecipeSentenceIngredientUpdateDto recipeSentenceIngredientUpdateDto)
        {
            if (recipeSentenceIngredientUpdateDto.RecipeSentenceIngredientId == null || recipeSentenceIngredientUpdateDto.IngredientIdOrNewIngredientName == null)
            {
                return BadRequest("Invalid Dto");
            }
            int changesMade = await _recipeRepository.UpdateRecipeSentenceIngredientAsync(recipeSentenceIngredientUpdateDto.RecipeSentenceIngredientId, recipeSentenceIngredientUpdateDto.IngredientIdOrNewIngredientName);
            if (changesMade == 0)
            {
                return BadRequest("No changes were made");
            }
            RecipeSentenceIngredient recipeSentenceIngredient = await _recipeRepository.GetRecipeSentenceIngredientAsync(recipeSentenceIngredientUpdateDto.RecipeSentenceIngredientId);
            return Ok(new { id = recipeSentenceIngredient.Id, ingredientId = recipeSentenceIngredient.Ingredient.Id, ingredientName = recipeSentenceIngredient.Ingredient.Name });
        }

        [HttpDelete("deleteRecipeSentenceIngredient/{id}")]
        public async Task<IActionResult> DeleteRecipeSentenceIngredientAsync(Guid id)
        {
            int changesMade = await _recipeRepository.DeleteRecipeSentenceIngredientAsync(id);
            if (changesMade == 0)
            {
                return BadRequest("No recipeSentenceIngredient found");
            }
            return Ok();
        }



    }
}
