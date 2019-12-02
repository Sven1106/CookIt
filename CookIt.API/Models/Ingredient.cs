using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookIt.API.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public string WordId { get; set; }
        public Ingredient()
        {

        }
        public Ingredient(string WordId)
        {
            this.Id = Guid.NewGuid();
            this.WordId = WordId;
        }
    }
}