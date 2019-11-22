using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Recipe
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string HostId { get; set; }
        public string Path { get; set; }
        public string ImageUrl { get; set; }

        public Recipe(string title, string hostId, string path, string imageUrl)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            HostId = hostId;
            Path = path;
            ImageUrl = imageUrl;
        }
    }
}
