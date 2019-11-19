using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Data
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string HostId { get; set; }
        public string Path { get; set; }
        public string ImageUrl { get; set; }

    }
}
