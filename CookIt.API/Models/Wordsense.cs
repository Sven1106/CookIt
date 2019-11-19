using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Wordsense
    {
        public string Id { get; set; }
        public string WordId { get; set; }
        public string SynsetId { get; set; }
        public string Register { get; set; }
    }
}
