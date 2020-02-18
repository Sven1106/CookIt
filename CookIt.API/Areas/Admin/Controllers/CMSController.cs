using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CookIt.API.Areas.Admin.Models;
using CookIt.API.Core;
using CookIt.API.Dtos;
using CookIt.API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CookIt.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CMSController : Controller
    {
        private readonly UnitOfWorkManager _unitOfWorkManager;
        public CMSController(IUnitOfWork unitOfWork)
        {
            _unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRecipes()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateRecipesAsync(string json)
        {
            CreateRecipeDto createRecipeDto = JsonConvert.DeserializeObject<CreateRecipeDto>(json);
            int rowsAdded = _unitOfWorkManager.CreateRecipes(createRecipeDto);
            return RedirectToAction("Recipes");
        }

        public IActionResult Recipes()
        {
            RecipesVm recipesVm = new RecipesVm(_unitOfWorkManager.GetRecipes(), _unitOfWorkManager.GetIngredients());
            return View(recipesVm);
        }
        public IActionResult EditRecipe(Guid id)
        {
            Recipe recipe = _unitOfWorkManager.GetRecipe(id);
            return View(recipe);
        }


    }
}