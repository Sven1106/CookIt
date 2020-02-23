using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using CookIt.API.Dtos;
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
        public IActionResult GetScaledImage([FromBody] ImageResizeDto imageResize)
        {
            string base64 = this._imageService.GetOrSetScaledImage(imageResize.Src, imageResize.Width, imageResize.Height);
            return Ok(base64);
        }
        [HttpDelete("DeleteImage")]
        public IActionResult DeleteImage([FromBody] ImageResizeDto imageResize)
        {
            bool imageDeleted = this._imageService.DeleteImage(HttpUtility.UrlDecode(imageResize.Src), imageResize.Width, imageResize.Height);
            if (imageDeleted == false)
            {
                return NoContent();
            }
            return Ok();
        }
        [HttpDelete("DeleteAllImages")]
        public IActionResult DeleteAllImages()
        {
            bool imagesDeleted = this._imageService.DeleteAllImages();
            if (imagesDeleted == false)
            {
                return NoContent();
            }
            return Ok();
        }
    }
}