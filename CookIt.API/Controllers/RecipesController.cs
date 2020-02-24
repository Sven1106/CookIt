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
        public RecipesController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpGet("getRecipes"), AllowAnonymous]
        public async Task<ActionResult> GetRecipesAsync([FromBody]RecipeFilter filter)
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

        [HttpPost("updateRecipeSentenceIngredient/{id}")]
        public async Task<IActionResult> UpdateRecipeSentenceIngredientAsync(Guid id, [FromBody] JObject json)
        {
            string ingredientValue = json["ingredientValue"].ToString();
            int changesMade = await _recipeRepository.UpdateRecipeSentenceIngredientAsync(id, ingredientValue);
            if (changesMade == 0)
            {
                return NoContent();
            }
            RecipeSentenceIngredient recipeSentenceIngredient = await _recipeRepository.GetRecipeSentenceIngredientAsync(id);
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
