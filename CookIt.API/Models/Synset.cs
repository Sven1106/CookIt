using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Synset
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public string Gloss { get; set; }
        public string Ontological_type { get; set; }
    }
}
