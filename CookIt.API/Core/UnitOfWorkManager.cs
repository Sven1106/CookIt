using CookIt.API.Dtos;
using CookIt.API.Models;
using CstLemmaLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CookIt.API.Core
{

    public static class IngredientErrors
    {
        public static Guid NoIngredientFound = Guid.Parse("00000000-0000-0000-0000-000000000000");
        public static Guid ToManyIngredientsFound = Guid.Parse("11111111-1111-1111-1111-111111111111");
    }
    public class UnitOfWorkManager
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfWorkManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int CreateRecipes(CreateRecipeDto createRecipeDto)
        {
            List<Ingredient> ingredients = _unitOfWork.IngredientRepo.Query.OrderBy(i => i.Name).ToList();

            Dictionary<string, List<string>> lemmasByIngredientName = CstLemmaWrapper.GetLemmasByTextDictionary(ingredients.Select(x => Helper.RemoveSpecialCharacters(x.Name)).ToList());
            Dictionary<Ingredient, List<string>> lemmasByIngredient = new Dictionary<Ingredient, List<string>>();
            foreach (var ingredient in ingredients)
            {
                var distinctLemmas = lemmasByIngredientName[Helper.RemoveSpecialCharacters(ingredient.Name)].Distinct().ToList();
                lemmasByIngredient.Add(ingredient, distinctLemmas);
            }

            Host host = _unitOfWork.HostRepo.Query.Where(h => h.Name == createRecipeDto.ProjectName).FirstOrDefault();
            if (host == null)
            {
                host = new Host(createRecipeDto.ProjectName, createRecipeDto.StartUrl, "https://via.placeholder.com/50");
                _unitOfWork.HostRepo.Insert(host);
            }

            foreach (var item in createRecipeDto.Task.AllRecipes)
            {
                //create recipe
                Recipe recipe = _unitOfWork.RecipeRepo.Query.Where(r => r.Host == host && r.Title == item.Recipe.Heading).FirstOrDefault();
                if (recipe == null) // CREATE
                {
                    Uri imageUrl;
                    if (item.Recipe.Image == null)
                    {
                        imageUrl = new Uri("https://via.placeholder.com/800x600");
                    }
                    else
                    {
                        imageUrl = item.Recipe.Image.Src;
                    }
                    recipe = new Recipe(item.Recipe.Heading, host, item.Metadata.FoundAtUrl, imageUrl);
                    _unitOfWork.RecipeRepo.Insert(recipe);


                    List<string> decodedRecipeIngredients = item.Recipe.Ingredients.Select(x => WebUtility.HtmlDecode(Helper.RemoveSpecialCharacters(x))).ToList();
                    List<string> destinctRecipeIngredients = decodedRecipeIngredients.Distinct().ToList();

                    Dictionary<string, List<string>> lemmasByRecipeIngredient = CstLemmaWrapper.GetLemmasByTextDictionary(destinctRecipeIngredients);
                    foreach (var lemmasByRecipeIngredientPair in lemmasByRecipeIngredient)
                    {

                        string recipeIngredientText = lemmasByRecipeIngredientPair.Key;
                        List<string> recipeIngredientDistinctLemmas = lemmasByRecipeIngredientPair.Value.Distinct().ToList();
                        Dictionary<Ingredient, List<string>> matchedLemmasByIngredient = new Dictionary<Ingredient, List<string>>();
                        foreach (KeyValuePair<Ingredient, List<string>> lemmasByIngredientPair in lemmasByIngredient)
                        {
                            List<string> ingredientLemmas = lemmasByIngredientPair.Value;
                            List<string> lemmaMatches = new List<string>();
                            for (int i = 0; i < recipeIngredientDistinctLemmas.Count; i++)
                            {
                                if (recipeIngredientDistinctLemmas[i] != recipeIngredientDistinctLemmas.Last()) // Handles whitespace by creating a compoundWord of nodes 
                                {
                                    string nextRecipeIngredientLemma = recipeIngredientDistinctLemmas[i + 1];
                                    foreach (var words in nextRecipeIngredientLemma.Split("|"))
                                    {
                                        string compoundOfLemmas = recipeIngredientDistinctLemmas[i].ToLower() + words.ToLower();
                                        string ingredientAsLemma = ingredientLemmas.FirstOrDefault().ToLower();
                                        int shortestEditDistance = Helper.GetShortestEditDistance(ingredientAsLemma, compoundOfLemmas);
                                        int editThreshold = 0;
                                        if (shortestEditDistance <= editThreshold)
                                        {
                                            lemmaMatches.Add(ingredientLemmas.FirstOrDefault());
                                            break;
                                        }
                                    }
                                }

                                foreach (var ingredientNode in ingredientLemmas)
                                {
                                    bool areLemmaAndLemmaEqual = recipeIngredientDistinctLemmas[i].ToLower() == ingredientNode.ToLower();
                                    if (areLemmaAndLemmaEqual)
                                    {
                                        lemmaMatches.Add(recipeIngredientDistinctLemmas[i]);
                                    }
                                }
                            }


                            if (lemmaMatches.Count > 0)
                            {
                                matchedLemmasByIngredient.Add(lemmasByIngredientPair.Key, lemmaMatches);
                            }
                        }
                        List<Ingredient> mostLikelyIngredients = new List<Ingredient>();
                        if (matchedLemmasByIngredient.Count == 0)
                        {
                            mostLikelyIngredients.Add(ingredients.Find(x => x.Id == IngredientErrors.NoIngredientFound));
                        }
                        else if (matchedLemmasByIngredient.Count > 0)
                        {
                            var ingredientsWithLemmaMatches = matchedLemmasByIngredient.Select(x => new
                            { //TODO Better names
                                Ingredient = x.Key,
                                IngredientLemmas = lemmasByIngredient[x.Key],
                                MatchedLemmas = x.Value
                            }).OrderBy(x => x.IngredientLemmas.Count - x.MatchedLemmas.Count).ToList();
                            var likelyIngredients = ingredientsWithLemmaMatches.Where(x => x.IngredientLemmas.Count == x.MatchedLemmas.Count).ToList();
                            if (likelyIngredients.Count == 0)
                            {
                                mostLikelyIngredients.Add(ingredients.Find(x => x.Id == IngredientErrors.NoIngredientFound));
                            }
                            else if (likelyIngredients.Count == 1)
                            {
                                mostLikelyIngredients.Add(likelyIngredients.FirstOrDefault().Ingredient);
                            }
                            else if (likelyIngredients.Count > 1)
                            {
                                mostLikelyIngredients.AddRange(likelyIngredients.Select(x => x.Ingredient).ToList());
                            }
                        }
                        RecipeSentence recipeIngredientForCreation = new RecipeSentence(recipe, recipeIngredientText);
                        _unitOfWork.RecipeSentenceRepo.Insert(recipeIngredientForCreation);

                        foreach (var mostLikelyIngredient in mostLikelyIngredients)
                        {
                            RecipeSentenceIngredient recipeSentenceIngredient = new RecipeSentenceIngredient(recipeIngredientForCreation, mostLikelyIngredient);
                            _unitOfWork.RecipeSentenceIngredientRepo.Insert(recipeSentenceIngredient);
                        }
                    }
                }
                else // UPDATE
                {

                }

            }

            return _unitOfWork.Complete();
        }


        public List<RecipeForListDto> GetRecipes(RecipeFilter filter)
        {
            List<Recipe> recipes = new List<Recipe>();
            IQueryable<Recipe> query = _unitOfWork.RecipeRepo.Query
                .AsNoTracking()
                .Include(recipe => recipe.Host)
                .Include(recipe => recipe.RecipeSentences)
                    .ThenInclude(recipeSentence => recipeSentence.RecipeSentenceIngredients)
                        .ThenInclude(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient);

            #region filtering
            if (filter.HostIds != null)
            {
                query = query.Where(x => filter.HostIds.Contains(x.Host.Id));
            }
            var bla = new List<Recipe>();
            if (filter.IngredientsIds != null)
            {
                query = query.Where(recipe =>
                                        recipe.RecipeSentences.Any(recipeSentence => recipeSentence.RecipeSentenceIngredients.Any(recipeSentenceIngredient => filter.IngredientsIds.Contains(recipeSentenceIngredient.Ingredient.Id))) &&
                                        (recipe.RecipeSentences.Count -
                                            recipe.RecipeSentences.Count(recipeSentence => 
                                                recipeSentence.RecipeSentenceIngredients.Any(x => filter.IngredientsIds.Contains(x.Ingredient.Id))
                                            ) <= filter.MissingIngredientsLimit
                                        ) &&
                                        recipe.RecipeSentences.Any(recipeSentence => recipeSentence.RecipeSentenceIngredients.Any(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient.Id == IngredientErrors.NoIngredientFound)) == false
                                    );
                bla = query.ToList();
            }
            #endregion

            #region DTOMapping
            List<RecipeForListDto> recipesForList = new List<RecipeForListDto>();
            recipesForList = query.Select(recipe => new RecipeForListDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Host = recipe.Host,
                Url = recipe.Url,
                ImageUrl = recipe.ImageUrl,
                Ingredients = recipe.RecipeSentences.SelectMany(recipeSentence => recipeSentence.RecipeSentenceIngredients.Select(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient)).ToList(), // THIS Flattens all the nested ingredients
                //MatchedIngredients = recipe.RecipeSentences.Select(x => x.RecpeSentenceIngredients).Where(y => filter.IngredientsIds != null ? filter.IngredientsIds.Contains(y.Id) : false).ToList()
            }).ToList();
            #endregion

            #region sorting
            recipesForList = recipesForList.OrderBy(x => x.Ingredients.Count - x.MatchedIngredients.Count)
                                    .ThenBy(x => x.Title).ToList();
            #endregion
            return recipesForList;
        }
        public Recipe GetRecipe(Guid id)
        {
            Recipe recipe = _unitOfWork.RecipeRepo.Query
                .AsNoTracking()
                .Include(recipe => recipe.Host)
                .Include(recipe => recipe.RecipeSentences)
                    .ThenInclude(recipeSentence => recipeSentence.RecipeSentenceIngredients)
                        .ThenInclude(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient)
                .Where(x => x.Id == id)
                .FirstOrDefault();
            return recipe;
        }
    }
}
