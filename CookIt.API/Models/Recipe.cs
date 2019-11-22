using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid HostId { get; set; }
        public string Path { get; set; }
        public string ImageUrl { get; set; }

        public Recipe(string title, Guid hostId, string path, string imageUrl)
        {
            Id = Guid.NewGuid();
            Title = title;
            HostId = hostId;
            Path = path;
            ImageUrl = imageUrl;
        }
    }
}
