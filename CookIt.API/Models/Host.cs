using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Models
{
    public class Host
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string LogoUrl { get; set; }
        public Host()
        {

        }
        public Host(string Name, string Url, string LogoUrl)
        {
            this.Id = Guid.NewGuid();
            this.Name = Name;
            this.Url = Url;
            this.LogoUrl = LogoUrl;
        }
    }
}
