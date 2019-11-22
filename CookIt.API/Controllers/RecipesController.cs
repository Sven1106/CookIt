using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Data;
using CookIt.API.Dtos;
using CookIt.API.Models;
using GenericServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Recipe = CookIt.API.Models.Recipe;

namespace CookIt.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private ICrudServices _service;

        public RecipesController(ICrudServices service)
        {
            _service = service;
        }
        [HttpPost]
        public async Task<ActionResult> CreateRecipes(RecipeJsonDto json)
        {
            Host host = _service.ReadSingle<Host>(Guid.NewGuid());
            if (host == null)
            {
                host = new Host(json.ProjectName, json.StartUrl.ToString(), "");
                _service.CreateAndSave(host);
            }
            foreach (var item in json.Data.AllRecipes)
            {

                Recipe recipeToCreate = new Recipe(item.Recipe.Heading, host.Id, item.Metadata.FoundAtUrl.ToString(), item.Recipe.Image.ToString());
                _service.CreateAndSave(recipeToCreate);
            }
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetRecipes()
        {
            var recipes = _service.ReadManyNoTracked<Recipe>().ToList();
            return Ok(recipes);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRecipe(Guid id)
        {
            return Ok();
        }
    }
}
