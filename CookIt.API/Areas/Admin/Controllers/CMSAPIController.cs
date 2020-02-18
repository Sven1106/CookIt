using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Core;
using CookIt.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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

        [HttpPost]
        public IActionResult UpdateRecipeSentenceIngredient([FromBody] JObject json)
        {
            try
            {
                Guid id = json["id"].ToObject<Guid>();
                Guid ingredientId = json["ingredientId"].ToObject<Guid>();

                bool wasUpdated = _unitOfWorkManager.UpdateRecipeSentenceIngredient(id, ingredientId);
                if(wasUpdated == false)
                {
                    return NoContent();
                }
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            return Ok();
        }
        [HttpPost("{id}")]
        public IActionResult DeleteRecipeSentenceIngredient(Guid id)
        {
            bool wasDeleted = _unitOfWorkManager.DeleteRecipeSentenceIngredient(id);
            if (wasDeleted == false)
            {
                return NoContent();
            }
            return Ok();
        }


    }
}