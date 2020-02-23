using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Dtos;
using CookIt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Recipe = CookIt.API.Models.Recipe;

namespace CookIt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly UnitOfWorkManager _unitOfWorkManager;
        public RecipesController(IUnitOfWork unitOfWork)
        {
            _unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
        }

        [HttpGet]
        public ActionResult GetRecipesAsync([FromBody]RecipeFilter filter)
        {
            List<RecipeForListDto> recipes = _unitOfWorkManager.GetFilteredRecipes(filter);
            if (recipes == null || recipes.Count == 0)
            {
                return NoContent();
            }
            return Ok(recipes);
        }
        [HttpGet("{id}")]
        public ActionResult GetRecipeAsync(Guid id)
        {
            Recipe recipe = _unitOfWorkManager.GetRecipe(id);
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
            int changesMade = _unitOfWorkManager.UpdateRecipeSentenceIngredient(id, ingredientValue);
            if (changesMade == 0)
            {
                return NoContent();
            }
            RecipeSentenceIngredient recipeSentenceIngredient = _unitOfWorkManager.GetRecipeSentenceIngredient(id);
            return Ok(new { id = recipeSentenceIngredient.Id, ingredientId = recipeSentenceIngredient.Ingredient.Id, ingredientName = recipeSentenceIngredient.Ingredient.Name });
        }

        [HttpDelete("DeleteRecipeSentenceIngredient/{id}")]
        public IActionResult DeleteRecipeSentenceIngredient(Guid id)
        {
            int changesMade = _unitOfWorkManager.DeleteRecipeSentenceIngredient(id);
            if (changesMade == 0)
            {
                return NoContent();
            }
            return Ok();
        }

    }
}
