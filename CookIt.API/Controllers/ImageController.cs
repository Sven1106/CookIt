using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using ImageScalerLib;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            this._imageService = imageService;
        }
        [HttpPost]
        public string Post(string url, int scaleToPercentage = 100)
        {
            return this._imageService.GetOrSetScaledImage(url, scaleToPercentage);
        }
    }
}