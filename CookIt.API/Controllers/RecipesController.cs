using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Dtos;
using CookIt.API.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}
