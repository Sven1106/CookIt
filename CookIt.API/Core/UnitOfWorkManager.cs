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
    public class UnitOfWorkManager
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfWorkManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<int> CreateRecipesAsync(CreateRecipeDto createRecipeDto)
        {
            List<Ingredient> ingredients = await _unitOfWork.IngredientRepo.Query.OrderBy(i => i.Name).ToListAsync();

            Dictionary<string, List<string>> ingredientsAsLemma = CstLemmaWrapper.GetLemmasByTextDictionary(
                ingredients.Select(x => Helper.RemoveSpecialCharacters(x.Name)).ToList()
            );
            for (int i = 0; i < ingredients.Count; i++)
            {
                ingredients[i].Lemmas = ingredientsAsLemma.Values.ElementAt(i);
            }

            Host host = await _unitOfWork.HostRepo.Query.FirstOrDefaultAsync(h => h.Name == createRecipeDto.ProjectName);
            if (host == null)
            {
                host = new Host(createRecipeDto.ProjectName, createRecipeDto.StartUrl, "https://via.placeholder.com/50");
                _unitOfWork.HostRepo.Insert(host);
            }

            foreach (var item in createRecipeDto.Task.AllRecipes)
            {
                //create recipe
                Recipe recipe = await _unitOfWork.RecipeRepo.Query.FirstOrDefaultAsync(r => r.Host == host && r.Title == item.Recipe.Heading);
                if (recipe == null) // CREATE
                {
                    recipe = new Recipe(item.Recipe.Heading, host, item.Metadata.FoundAtUrl, item.Recipe.Image.Src);
                    _unitOfWork.RecipeRepo.Insert(recipe);


                    List<string> decodedRecipeIngredients = item.Recipe.Ingredients.Select(x => WebUtility.HtmlDecode(Helper.RemoveSpecialCharacters(x))).ToList();
                    List<string> destinctRecipeIngredients = decodedRecipeIngredients.Distinct().ToList();

                    Dictionary<string, List<string>> lemmasByRecipeIngredient = CstLemmaWrapper.GetLemmasByTextDictionary(destinctRecipeIngredients);
                    foreach (var lemmasByRecipeIngredientPair in lemmasByRecipeIngredient)
                    {

                        string recipeIngredientText = lemmasByRecipeIngredientPair.Key;
                        List<string> recipeIngredientLemmas = lemmasByRecipeIngredientPair.Value;
                        Dictionary<Ingredient, List<string>> matchedNodesByIngredient = new Dictionary<Ingredient, List<string>>();
                        foreach (Ingredient ingredient in ingredients)
                        {
                            List<string> ingredientLemmas = ingredient.Lemmas;
                            List<string> lemmaMatches = new List<string>();
                            for (int i = 0; i < recipeIngredientLemmas.Count; i++)
                            {
                                if (recipeIngredientLemmas[i] != recipeIngredientLemmas.Last()) // Handles whitespace by creating a compoundWord of nodes 
                                {
                                    string nextRecipeIngredientLemma = recipeIngredientLemmas[i + 1];
                                    foreach (var words in nextRecipeIngredientLemma.Split("|"))
                                    {
                                        string compoundOfLemmas = recipeIngredientLemmas[i].ToLower() + words.ToLower();
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
                                    bool areLemmaAndLemmaEqual = recipeIngredientLemmas[i].ToLower() == ingredientNode.ToLower();
                                    if (areLemmaAndLemmaEqual)
                                    {
                                        lemmaMatches.Add(recipeIngredientLemmas[i]);
                                    }
                                }
                            }


                            if (lemmaMatches.Count > 0)
                            {
                                matchedNodesByIngredient.Add(ingredient, lemmaMatches);
                            }
                        }
                        Ingredient mostLikelyIngredient = new Ingredient();
                        if (matchedNodesByIngredient.Count == 0)
                        {
                            mostLikelyIngredient = ingredients.Find(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000000"));
                        }
                        else if (matchedNodesByIngredient.Count == 1)
                        {
                            mostLikelyIngredient = ingredients.Find(x => x.Id == matchedNodesByIngredient.FirstOrDefault().Key.Id);
                        }
                        else if (matchedNodesByIngredient.Count > 1)
                        {
                            int nodeCountOfMostMatched = matchedNodesByIngredient.OrderByDescending(x => x.Value.Count).ToList().FirstOrDefault().Value.Count();
                            var ingredientsWithMostNodeMatches = matchedNodesByIngredient.Where(x => x.Value.Count == nodeCountOfMostMatched).ToList();
                            var likelyIngredients = ingredientsWithMostNodeMatches.Where(x => x.Key.Lemmas.Count == x.Value.Count).ToList();
                            if (likelyIngredients.Count == 0)
                            {
                                mostLikelyIngredient = ingredients.Find(x => x.Id == Guid.Parse("00000000-0000-0000-0000-000000000000"));
                            }
                            else if (likelyIngredients.Count == 1)
                            {
                                mostLikelyIngredient = ingredients.Find(x => x.Id == likelyIngredients.FirstOrDefault().Key.Id);
                            }
                            else if (likelyIngredients.Count > 1)
                            {
                                mostLikelyIngredient = ingredients.Find(x => x.Id == Guid.Parse("11111111-1111-1111-1111-111111111111"));
                            }
                        }
                        RecipeIngredient recipeIngredientForCreation = new RecipeIngredient(recipe, mostLikelyIngredient, recipeIngredientText);
                        _unitOfWork.RecipeIngredientRepo.Insert(recipeIngredientForCreation);
                    }
                }
                else // UPDATE
                {

                }
            }
            return _unitOfWork.Complete();
        }


        public async Task<List<Recipe>> GetRecipesAsync(RecipeFilter filter)
        {
            List<Recipe> recipes = await _unitOfWork.RecipeRepo.Query
                 .Include(recipe => recipe.Host)
                 .Include(recipe => recipe.RecipeIngredients)
                     .ThenInclude(i => i.Ingredient)
                 .ToListAsync();
            if (filter.IngredientsIds != null)
            {
                recipes = recipes.Where(x =>
                    x.RecipeIngredients.Any(y =>
                        filter.IngredientsIds.Contains(y.Ingredient.Id))
                    ).ToList();
            }
            return recipes;
        }
        public async Task<Recipe> GetRecipeAsync(Guid id)
        {
            Recipe recipe = await _unitOfWork.RecipeRepo.Query
                .Include(recipe => recipe.Host)
                .Include(recipe => recipe.RecipeIngredients)
                    .ThenInclude(i => i.Ingredient)
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            return recipe;
        }
    }
}
