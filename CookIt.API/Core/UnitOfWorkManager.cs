using CookIt.API.Dtos;
using CookIt.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;

namespace CookIt.API.Core
{
    public class UnitOfWorkManager
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnitOfWorkManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public int CreateRecipes(CreateRecipeDto createRecipeDto)
        {
            IEnumerable<Word> words = _unitOfWork.WordRepo.Query.OrderBy(w => w.LexicalForm).ToList();
            Host host = _unitOfWork.HostRepo.Query.FirstOrDefault(h => h.Name == createRecipeDto.ProjectName);
            if (host == null)
            {
                host = new Host(createRecipeDto.ProjectName, createRecipeDto.StartUrl, "https://via.placeholder.com/50");
                _unitOfWork.HostRepo.Insert(host);
            }

            foreach (var item in createRecipeDto.Data.AllRecipes)
            {
                //create recipe
                Recipe recipe = _unitOfWork.RecipeRepo.Query.FirstOrDefault(r => r.HostId == host.Id && r.Title == item.Recipe.Heading);
                if (recipe == null) // CREATE
                {
                    recipe = new Recipe(item.Recipe.Heading, host.Id, item.Metadata.FoundAtUrl, item.Recipe.Image.Src);
                    _unitOfWork.RecipeRepo.Insert(recipe);
                    foreach (var ingredientSentence in item.Recipe.Ingredients)
                    {
                        string ingredientSentenceDecodedToLower = WebUtility.HtmlDecode(ingredientSentence).ToLower();
                        Word mostLikelyWord = new Word();

                        IList<Word> matchingWords = new List<Word>();
                        // FIRST Boyer–Moore string-search
                        foreach (Word word in words)
                        {
                            string wordToLower = word.LexicalForm.ToLower(); // A bit Naive. TODO Find a better way to handle whitespace
                            if (Helper.FindPatternInString(ingredientSentenceDecodedToLower, wordToLower).Length > 0)
                            {
                                matchingWords.Add(word);
                            }
                        }
                        Dictionary<Word, int> matchingwordToEditDistance = new Dictionary<Word, int>();
                        foreach (Word matchingWord in matchingWords)
                        {
                            string matchingWordToLower = matchingWord.LexicalForm.ToLower(); // A bit Naive. TODO Find a better way to handle whitespace
                            int shortestDistance = Helper.GetShortestEditDistance(matchingWordToLower, ingredientSentenceDecodedToLower);
                            matchingwordToEditDistance.Add(matchingWord, shortestDistance);
                        }
                        if (matchingwordToEditDistance.Count() > 0)
                        {
                            mostLikelyWord = matchingwordToEditDistance.Aggregate((l, r) => l.Value < r.Value ? l : r).Key;

                            Ingredient ingredient = _unitOfWork.IngredientRepo.Query.FirstOrDefault(i => i.WordId == mostLikelyWord.Id);
                            if (ingredient == null)
                            {
                                ingredient = new Ingredient(mostLikelyWord.Id);
                                _unitOfWork.IngredientRepo.Insert(ingredient);
                            }

                            if (_unitOfWork.RecipeIngredientRepo.Query.FirstOrDefault(ri => ri.IngredientId == ingredient.Id) == null)
                            {
                                RecipeIngredient recipeIngredient = new RecipeIngredient(recipe.Id, ingredient.Id, ingredientSentenceDecodedToLower);
                                _unitOfWork.RecipeIngredientRepo.Insert(recipeIngredient);
                            }
                        }
                        else
                        {
                            //HANDLE NO MATCHING WORDS WERE FOUND.
                            RecipeIngredient recipeIngredient = new RecipeIngredient(recipe.Id, Guid.Empty, ingredientSentenceDecodedToLower);
                            _unitOfWork.RecipeIngredientRepo.Insert(recipeIngredient);
                            //throw new Exception("NO MATCHING WORDS WERE FOUND.");

                        }


                    }
                }
                else // UPDATE
                {

                }
            }
            return _unitOfWork.Complete();
        }
    }
}
