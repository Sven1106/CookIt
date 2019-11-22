using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Host
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public Host(string name, string url, string logoUrl)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Url = url;
            LogoUrl = logoUrl;
        }
    }
}
