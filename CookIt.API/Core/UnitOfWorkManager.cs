﻿using CookIt.API.Dtos;
using CookIt.API.Models;
using CstLemmaLibrary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace CookIt.API.Core
{
    public static class IngredientErrors
    {
        public static Guid NoIngredientFound = Guid.Parse("00000000-0000-0000-0000-000000000000");
    }

    public class UnitOfWorkManager
    {
        private readonly IUnitOfWork _unitOfWork;

        public UnitOfWorkManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public List<Ingredient> GetIngredients()
        {
            List<Ingredient> ingredients = _unitOfWork.IngredientRepo.Query
               .AsNoTracking()
               .ToList();
            return ingredients;
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
                host = new Host(createRecipeDto.ProjectName, createRecipeDto.Domain, ImageScalerLib.ImageService.CreatePlaceholderImage(50,50));
                _unitOfWork.HostRepo.Insert(host);
            }

            foreach (var item in createRecipeDto.Tasks.AllRecipes)
            {
                //create recipe
                Recipe recipe = _unitOfWork.RecipeRepo.Query.Where(r => r.Host == host && r.Title == item.Recipe.Heading).FirstOrDefault();
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

        public List<Recipe> GetRecipes()
        {
            List<Recipe> recipes = _unitOfWork.RecipeRepo.Query
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
            Recipe recipe = _unitOfWork.RecipeRepo.Query.Where(x => x.Id == id).FirstOrDefault();
            if (recipe == null)
            {
                return 0;
            }
            List<RecipeSentence> recipeSentences = _unitOfWork.RecipeSentenceRepo.Query
                .Where(x => x.Recipe == recipe)
                .ToList();
            foreach (var recipeSentence in recipeSentences)
            {
                List<RecipeSentenceIngredient> recipeSentenceIngredients = _unitOfWork.RecipeSentenceIngredientRepo.Query
                    .Where(x => x.RecipeSentence == recipeSentence)
                    .ToList();
                foreach (var recipeSentenceIngredient in recipeSentenceIngredients)
                {
                    _unitOfWork.RecipeSentenceIngredientRepo.Delete(recipeSentenceIngredient);
                }
                _unitOfWork.RecipeSentenceRepo.Delete(recipeSentence);
            }
            _unitOfWork.RecipeRepo.Delete(recipe);
            return _unitOfWork.Complete();
        }

        public RecipeSentenceIngredient GetRecipeSentenceIngredient(Guid id)
        {
            RecipeSentenceIngredient recipeSentenceIngredient = _unitOfWork.RecipeSentenceIngredientRepo.Query
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

            RecipeSentenceIngredient recipeSentenceIngredient = _unitOfWork.RecipeSentenceIngredientRepo.Query.Where(x => x.Id == id).FirstOrDefault();
            if (recipeSentenceIngredient == null)
            {
                return 0;
            }
            Ingredient ingredient;
            if (Guid.TryParse(ingredientValue, out Guid ingredientId))
            {
                ingredient = _unitOfWork.IngredientRepo.Query.Where(x => x.Id == ingredientId).FirstOrDefault();
            }
            else
            {
                ingredient = _unitOfWork.IngredientRepo.Query.Where(x => x.Name == ingredientValue).FirstOrDefault();
            }
            if (ingredient == null)
            {
                ingredient = new Ingredient(ingredientValue);
                _unitOfWork.IngredientRepo.Insert(ingredient);
            }
            recipeSentenceIngredient.Ingredient = ingredient;
            return _unitOfWork.Complete();
        }

        public int DeleteRecipeSentenceIngredient(Guid id)
        {
            RecipeSentenceIngredient recipeSentenceIngredient = _unitOfWork.RecipeSentenceIngredientRepo.Query.Where(x => x.Id == id).FirstOrDefault();
            if (recipeSentenceIngredient == null)
            {
                return 0;
            }
            _unitOfWork.RecipeSentenceIngredientRepo.Delete(recipeSentenceIngredient);
            return _unitOfWork.Complete();
        }

        #region Auth
        public User Login(string username, string password)
        {
            var user = this._unitOfWork.AuthRepository.Query.Where(x => x.Username == username).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public User Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            this._unitOfWork.AuthRepository.Insert(user);
            this._unitOfWork.Complete();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

        }
        public bool UserExists(string username)
        {
            if (this._unitOfWork.AuthRepository.Query.Any(x => x.Username == username))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}