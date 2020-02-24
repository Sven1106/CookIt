using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Dtos;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Recipe = CookIt.API.Models.Recipe;

namespace CookIt.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "", Roles = Role.Admin)]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipesController(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        [HttpGet, AllowAnonymous]
        public ActionResult GetRecipesAsync([FromBody]RecipeFilter filter)
        {
            List<RecipeForListDto> recipes = _recipeRepository.GetFilteredRecipes(filter);
            if (recipes == null || recipes.Count == 0)
            {
                return NoContent();
            }
            return Ok(recipes);
        }
        [HttpGet("{id}"), AllowAnonymous]
        public ActionResult GetRecipeAsync(Guid id)
        {
            Recipe recipe = _recipeRepository.GetRecipe(id);
            if (recipe == null)
            {
                return NoContent();
            }
            return Ok(recipe);
        }

        [HttpPost("UpdateRecipeSentenceIngredient/{id}")]
        public IActionResult UpdateRecipeSentenceIngredient(Guid id, [FromBody] JObject json)
        {
            string ingredientValue = json["ingredientValue"].ToString();
            int changesMade = _recipeRepository.UpdateRecipeSentenceIngredient(id, ingredientValue);
            if (changesMade == 0)
            {
                return NoContent();
            }
            RecipeSentenceIngredient recipeSentenceIngredient = _recipeRepository.GetRecipeSentenceIngredient(id);
            return Ok(new { id = recipeSentenceIngredient.Id, ingredientId = recipeSentenceIngredient.Ingredient.Id, ingredientName = recipeSentenceIngredient.Ingredient.Name });
        }

        [HttpDelete("DeleteRecipeSentenceIngredient/{id}")]
        public IActionResult DeleteRecipeSentenceIngredient(Guid id)
        {
            int changesMade = _recipeRepository.DeleteRecipeSentenceIngredient(id);
            if (changesMade == 0)
            {
                return NoContent();
            }
            return Ok();
        }

    }
}
