using CookIt.API.Core;
using CookIt.API.Dtos;
using ImageScalerLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "", Roles = Role.Admin)]
    public class ImageController : ControllerBase
    {
        private readonly ImageService _imageService;
        public ImageController(ImageService imageService)
        {
            this._imageService = imageService;
        }
        [AllowAnonymous]
        [HttpPost("GetScaledImage")]
        public IActionResult GetScaledImage(ImageResizeDto imageResize)
        {
            string base64 = this._imageService.GetOrSetScaledImage(imageResize.Src, imageResize.Width, imageResize.Height);
            return Ok(base64);
        }
        [HttpDelete("DeleteImage")]
        public IActionResult DeleteImage(ImageResizeDto imageResize)
        {
            bool imageDeleted = this._imageService.DeleteImage(imageResize.Src, imageResize.Width, imageResize.Height);
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