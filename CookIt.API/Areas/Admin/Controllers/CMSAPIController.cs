using CookIt.API.Core;
using CookIt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;

namespace CookIt.API.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CMSAPIController : ControllerBase
    {
        private readonly UnitOfWorkManager _unitOfWorkManager;

        public CMSAPIController(IUnitOfWork unitOfWork)
        {
            _unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
        }

        [Route("UpdateRecipeSentenceIngredient")]
        [HttpPost]
        public IActionResult UpdateRecipeSentenceIngredient([FromBody] JObject json)
        {
            Guid id = json["recipeSentenceIngredientId"].ToObject<Guid>();
            string ingredientValue = json["ingredientValue"].ToString();

            int changesMade = _unitOfWorkManager.UpdateRecipeSentenceIngredient(id, ingredientValue);
            if (changesMade == 0)
            {
                return NoContent();
            }
            RecipeSentenceIngredient recipeSentenceIngredient = _unitOfWorkManager.GetRecipeSentenceIngredient(id);
            return Ok(new { id = recipeSentenceIngredient.Id, ingredientId = recipeSentenceIngredient.Ingredient.Id, ingredientName = recipeSentenceIngredient.Ingredient.Name });
        }

        [HttpPost("{id}")]
        [Route("DeleteRecipeSentenceIngredient")]
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