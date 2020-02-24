using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CookIt.API.Dtos
{
    public class ImageResizeDto
    {
        [Required]
        public string Src { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
