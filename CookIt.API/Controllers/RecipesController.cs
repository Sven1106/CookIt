using System;
using System.Collections.Generic;
using System.Linq;
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
            List<Ingredient> ingredients =  await _ingredientRepository.GetIngredients();
            if (ingredients == null || ingredients.Count == 0)
            {
                return NoContent();
            }
            return Ok(ingredients);
        }
        [HttpGet("getRecipes"), AllowAnonymous]
        public async Task<ActionResult> GetRecipesAsync([FromQuery]RecipeFilter filter)
        {
            List<RecipeForListDto> recipes = await _recipeRepository.GetFilteredRecipesAsync(filter);
            if (recipes == null || recipes.Count == 0)
            {
                return NoContent();
            }
            return Ok(recipes);
        }
        [HttpGet("{id}"), AllowAnonymous]
        public async Task<ActionResult> GetRecipeAsync(Guid id)
        {
            Recipe recipe = await _recipeRepository.GetRecipeAsync(id);
            if (recipe == null)
            {
                return NoContent();
            }
            return Ok(recipe);
        }

        [HttpPost("updateRecipeSentenceIngredient")]
        public async Task<IActionResult> UpdateRecipeSentenceIngredientAsync(RecipeSentenceIngredientUpdateDto recipeSentenceIngredientUpdateDto)
        {
            if (recipeSentenceIngredientUpdateDto.RecipeSentenceIngredientId == null || recipeSentenceIngredientUpdateDto.IngredientIdOrNewIngredientName == null)
            {
                return BadRequest();
            }
            int changesMade = await _recipeRepository.UpdateRecipeSentenceIngredientAsync(recipeSentenceIngredientUpdateDto.RecipeSentenceIngredientId, recipeSentenceIngredientUpdateDto.IngredientIdOrNewIngredientName);
            if (changesMade == 0)
            {
                return NoContent();
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
                return NoContent();
            }
            return Ok();
        }



    }
}
