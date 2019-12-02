using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Recipe
    {

        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid HostId { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public Recipe()
        {

        }
        public Recipe(string Title, Guid HostId, string Url, string ImageUrl)
        {
            this.Id = Guid.NewGuid();
            this.Title = Title;
            this.HostId = HostId;
            this.Url = Url;
            this.ImageUrl = ImageUrl;
        }
    }
}
