using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CookIt.API.Areas.Admin.Models;
using CookIt.API.Core;
using CookIt.API.Dtos;
using CookIt.API.Models;
using ImageScalerLib;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace CookIt.API.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CMSController : Controller
    {
        private readonly UnitOfWorkManager _unitOfWorkManager;
        private readonly ImageService _imageService;
        private string _currentDirectory;
        private JSchema _createRecipeSchema;
        public CMSController(IUnitOfWork unitOfWork, ImageService imageService)
        {
            _unitOfWorkManager = new UnitOfWorkManager(unitOfWork);
            _currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _createRecipeSchema = JSchema.Parse(System.IO.File.ReadAllText(Path.Combine(_currentDirectory, "createRecipeSchema.json")));
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateRecipes()
        {
            return View(_createRecipeSchema);
        }
        [HttpPost]
        public IActionResult CreateRecipesAsync(string json)
        {
            JSchemaValidatingReader jSchemaReader = new JSchemaValidatingReader(new JsonTextReader(new StringReader(json)));
            jSchemaReader.Schema = _createRecipeSchema;

            IList<string> errorMessages = new List<string>();
            jSchemaReader.ValidationEventHandler += (o, a) => errorMessages.Add(a.Message);
            JsonSerializer serializer = new JsonSerializer();
            CreateRecipeDto createRecipeDto = serializer.Deserialize<CreateRecipeDto>(jSchemaReader);
            if (errorMessages.Count > 0)
            {
                foreach (var eventMessage in errorMessages)
                {
                }
            }




            int rowsAdded = _unitOfWorkManager.CreateRecipes(createRecipeDto);
            return RedirectToAction("Recipes");
        }

        public IActionResult Recipes()
        {
            RecipesVm recipesVm = new RecipesVm(_unitOfWorkManager.GetRecipes(), _unitOfWorkManager.GetIngredients());
            return View(recipesVm);
        }
        public IActionResult DeleteRecipe(Guid id)
        {
            _unitOfWorkManager.DeleteRecipe(id);

            return RedirectToAction("Recipes");
        }


    }
}