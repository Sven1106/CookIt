using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookIt.API.Models
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Ingredient()
        {

        }
        public Ingredient(string name)
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
        }
    }
}