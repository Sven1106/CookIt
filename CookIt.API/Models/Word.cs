using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Word
    {
        public string Id { get; set; }
        public string LexicalForm { get; set; }
        public string Pos { get; set; }
    }
}
