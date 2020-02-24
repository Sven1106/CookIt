using CookIt.API.Core;
using CookIt.API.Data;
using CookIt.API.Dtos;
using CookIt.API.Interfaces;
using CookIt.API.Models;
using CstLemmaLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CookIt.API.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly AppDbContext _appDbContext;
        public RecipeRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public int CreateRecipes(CreateRecipeDto createRecipeDto)
        {
            List<Ingredient> ingredients = _appDbContext.Ingredient.OrderBy(i => i.Name).ToList();
            Dictionary<string, List<string>> lemmasByIngredientName = CstLemmaWrapper.GetLemmasByTextDictionary(ingredients.Select(x => Helper.RemoveSpecialCharacters(x.Name)).ToList());
            Dictionary<Ingredient, List<string>> lemmasByIngredient = new Dictionary<Ingredient, List<string>>();
            foreach (var ingredient in ingredients)
            {
                var distinctLemmas = lemmasByIngredientName[Helper.RemoveSpecialCharacters(ingredient.Name)].Distinct().ToList();
                lemmasByIngredient.Add(ingredient, distinctLemmas);
            }

            Host host = _appDbContext.Host.Where(h => h.Name == createRecipeDto.ProjectName).FirstOrDefault();
            if (host == null)
            {
                host = new Host(createRecipeDto.ProjectName, createRecipeDto.Domain, ImageScalerLib.ImageService.CreatePlaceholderImage(50, 50));
                _appDbContext.Host.Add(host);
            }

            foreach (var item in createRecipeDto.Tasks.AllRecipes)
            {
                //create recipe
                Recipe recipe = _appDbContext.Recipe.Where(r => r.Host == host && r.Title == item.Recipe.Heading).FirstOrDefault();
                if (recipe == null) // CREATE
                {
                    string imageUrl;
                    if (item.Recipe.Image == null)
                    {
                        imageUrl = "https://via.placeholder.com/800x600";
                    }
                    else
                    {
                        imageUrl = item.Recipe.Image.Src;
                    }
                    recipe = new Recipe(item.Recipe.Heading, host, item.Metadata.FoundAtUrl, imageUrl);
                    _appDbContext.Recipe.Add(recipe);

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
                                    foreach (var word in nextRecipeIngredientLemma.Split("|")) // handles lemmas returned with wordform
                                    {
                                        string compoundOfLemmas = recipeIngredientDistinctLemmas[i].ToLower() + word.ToLower();
                                        string ingredientAsLemma = ingredientLemmas.FirstOrDefault().ToLower();
                                        if (ingredientAsLemma == compoundOfLemmas)
                                        {
                                            lemmaMatches.Add(ingredientLemmas.FirstOrDefault());
                                            break;
                                        }
                                    }
                                }

                                foreach (var ingredientLemma in ingredientLemmas)
                                {
                                    var ingredientLemmaSplit = ingredientLemma.ToLower().Split("|"); // handles lemmas returned with wordform
                                    var recipeIngredientDistinctLemmasSplit = recipeIngredientDistinctLemmas[i].ToLower().Split("|"); // handles lemmas returned with wordform
                                    bool doesLemmasIntersect = recipeIngredientDistinctLemmasSplit.Intersect(ingredientLemmaSplit).Any();
                                    if (doesLemmasIntersect)
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
                            var ingredientsWhereLemmasCountAreEqualOrderedByCountDesc = ingredientsWithLemmaMatches.Where(x => x.IngredientLemmas.Count == x.MatchedLemmas.Count).OrderByDescending(x => x.MatchedLemmas.Count).ToList();
                            var likelyIngredients = ingredientsWhereLemmasCountAreEqualOrderedByCountDesc.Where(x => x.MatchedLemmas.Count == ingredientsWhereLemmasCountAreEqualOrderedByCountDesc.FirstOrDefault().MatchedLemmas.Count).ToList();
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
                        _appDbContext.RecipeSentence.Add(recipeIngredientForCreation);

                        foreach (var mostLikelyIngredient in mostLikelyIngredients)
                        {
                            RecipeSentenceIngredient recipeSentenceIngredient = new RecipeSentenceIngredient(recipeIngredientForCreation, mostLikelyIngredient);
                            _appDbContext.RecipeSentenceIngredient.Add(recipeSentenceIngredient);
                        }
                    }
                }
                else // UPDATE
                {
                }
            }

            return _appDbContext.SaveChanges();
        }

        public Recipe GetRecipe(Guid id)
        {
            Recipe recipe = _appDbContext.Recipe
                .AsNoTracking()
                .Include(recipe => recipe.Host)
                .Include(recipe => recipe.RecipeSentences)
                    .ThenInclude(recipeSentence => recipeSentence.RecipeSentenceIngredients)
                        .ThenInclude(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient)
                .Where(x => x.Id == id)
                .FirstOrDefault();
            return recipe;
        }

        public List<Recipe> GetRecipes()
        {
            List<Recipe> recipes = _appDbContext.Recipe
                .AsNoTracking()
                .Include(recipe => recipe.Host)
                .Include(recipe => recipe.RecipeSentences)
                    .ThenInclude(recipeSentence => recipeSentence.RecipeSentenceIngredients)
                        .ThenInclude(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient).ToList();
            return recipes;
        }

        public List<RecipeForListDto> GetFilteredRecipes(RecipeFilter filter)
        {
            List<Recipe> recipes = new List<Recipe>();

            IQueryable<Recipe> query = _appDbContext.Recipe
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
            if (filter.IngredientsIds != null)
            {
                query = query
                    .Where(recipe => recipe.RecipeSentences
                        .Any(recipeSentence => recipeSentence.RecipeSentenceIngredients
                            .Any(recipeSentenceIngredient => filter.IngredientsIds
                                .Contains(recipeSentenceIngredient.Ingredient.Id)
                            )
                        )
                        && (recipe.RecipeSentences.Count - recipe.RecipeSentences
                            .Count(recipeSentence => recipeSentence.RecipeSentenceIngredients
                                .Any(x => filter.IngredientsIds.Contains(x.Ingredient.Id))
                            ) <= filter.MissingIngredientsLimit
                        )
                        && recipe.RecipeSentences
                            .Any(recipeSentence => recipeSentence.RecipeSentenceIngredients
                                .Any(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient.Id == IngredientErrors.NoIngredientFound)
                            ) == false
                    );
            }

            #endregion filtering

            #region DTOMapping

            List<RecipeForListDto> filteredRecipesForList = new List<RecipeForListDto>();
            filteredRecipesForList = query
                .Select(recipe => new RecipeForListDto
                {
                    Id = recipe.Id,
                    Title = recipe.Title,
                    Host = recipe.Host,
                    Url = recipe.Url,
                    ImageUrl = recipe.ImageUrl,
                    Ingredients = recipe.RecipeSentences
                        .Select(recipeSentence => recipeSentence.RecipeSentenceIngredients
                            .Any(x => filter.IngredientsIds != null ? filter.IngredientsIds.Contains(x.Ingredient.Id) : false)
                            ? recipeSentence.RecipeSentenceIngredients
                                .Select(x => x.Ingredient)
                                .Where(x => filter.IngredientsIds != null
                                    ? filter.IngredientsIds.Contains(x.Id)
                                    : false
                                )
                                .FirstOrDefault()
                            : recipeSentence.RecipeSentenceIngredients
                                .Select(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient)
                                .FirstOrDefault()
                        )
                        .ToList(),
                    MatchedIngredients = recipe.RecipeSentences
                        .Select(recipeSentence => recipeSentence.RecipeSentenceIngredients
                            .Select(x => x.Ingredient)
                            .Where(x => filter.IngredientsIds != null
                                ? filter.IngredientsIds.Contains(x.Id)
                                : false)
                            .FirstOrDefault()
                        )
                        .AsQueryable()
                        .Where(x => x != null)
                        .ToList()
                }).ToList();

            #endregion DTOMapping

            #region sorting

            filteredRecipesForList = filteredRecipesForList
                .OrderBy(x => x.Ingredients.Count - x.MatchedIngredients.Count)
                .ThenBy(x => x.Title)
                .ToList();

            #endregion sorting

            return filteredRecipesForList;
        }

        public int DeleteRecipe(Guid id)
        {
            Recipe recipe = _appDbContext.Recipe.Where(x => x.Id == id).FirstOrDefault();
            if (recipe == null)
            {
                return 0;
            }
            List<RecipeSentence> recipeSentences = _appDbContext.RecipeSentence
                .Where(x => x.Recipe == recipe)
                .ToList();
            foreach (var recipeSentence in recipeSentences)
            {
                List<RecipeSentenceIngredient> recipeSentenceIngredients = _appDbContext.RecipeSentenceIngredient
                    .Where(x => x.RecipeSentence == recipeSentence)
                    .ToList();
                foreach (var recipeSentenceIngredient in recipeSentenceIngredients)
                {
                    _appDbContext.RecipeSentenceIngredient.Remove(recipeSentenceIngredient);
                }
                _appDbContext.RecipeSentence.Remove(recipeSentence);
            }
            _appDbContext.Recipe.Remove(recipe);
            return _appDbContext.SaveChanges();
        }

        public RecipeSentenceIngredient GetRecipeSentenceIngredient(Guid id)
        {
            RecipeSentenceIngredient recipeSentenceIngredient = _appDbContext.RecipeSentenceIngredient
                .AsNoTracking()
                .Include(recipeSentenceIngredient => recipeSentenceIngredient.RecipeSentence) //TODO WHY IS THIS NOT INCLUDED??!
                .Include(recipeSentenceIngredient => recipeSentenceIngredient.Ingredient)
                .Where(x => x.Id == id)
                .FirstOrDefault();
            return recipeSentenceIngredient;
        }

        public int UpdateRecipeSentenceIngredient(Guid id, string ingredientValue)
        {
            if (ingredientValue == "")
            {
                return 0;
            }

            RecipeSentenceIngredient recipeSentenceIngredient = _appDbContext.RecipeSentenceIngredient.Where(x => x.Id == id).FirstOrDefault();
            if (recipeSentenceIngredient == null)
            {
                return 0;
            }
            Ingredient ingredient;
            if (Guid.TryParse(ingredientValue, out Guid ingredientId))
            {
                ingredient = _appDbContext.Ingredient.Where(x => x.Id == ingredientId).FirstOrDefault();
            }
            else
            {
                ingredient = _appDbContext.Ingredient.Where(x => x.Name == ingredientValue).FirstOrDefault();
            }
            if (ingredient == null)
            {
                ingredient = new Ingredient(ingredientValue);
                _appDbContext.Ingredient.Add(ingredient);
            }
            recipeSentenceIngredient.Ingredient = ingredient;
            return _appDbContext.SaveChanges();
        }

        public int DeleteRecipeSentenceIngredient(Guid id)
        {
            RecipeSentenceIngredient recipeSentenceIngredient = _appDbContext.RecipeSentenceIngredient.Where(x => x.Id == id).FirstOrDefault();
            if (recipeSentenceIngredient == null)
            {
                return 0;
            }
            _appDbContext.RecipeSentenceIngredient.Remove(recipeSentenceIngredient);
            return _appDbContext.SaveChanges();
        }
    }
}
