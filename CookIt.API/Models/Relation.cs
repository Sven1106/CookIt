using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Relation
    {
        public string FromSynsetId { get; set; }
        public string Name { get; set; }
        public string Name2 { get; set; }
        public string ToSynsetId { get; set; }
        public string Taxonomic { get; set; }
        public string Inheritance_comment { get; set; }

    }
}
