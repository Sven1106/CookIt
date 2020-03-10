using CookIt.API.Models;
using CookIt.API.Dtos;
using ImageScalerLib;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CookIt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    /*
        This is a mixed Authorized Controller. It can be authorized with cookies or JWT.
        The Ajax requests made to this controller are from same origin which means the cookieauthorization is used.
    */
    [Authorize(AuthenticationSchemes = AuthSchemes, Policy = "", Roles = Role.Admin)]
    public class ImageController : ControllerBase
    {
        private const string AuthSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme; // Authorizes against both Schemes.
        private readonly ImageService _imageService;
        public ImageController(ImageService imageService)
        {
            this._imageService = imageService;
        }
        [HttpPost("getImage")]
        public async Task<IActionResult> GetScaledImageAsync(ImageResizeDto imageResize)
        {
            string base64 = await this._imageService.GetOrSetScaledImageAsync(imageResize.Src, imageResize.Width, imageResize.Height);
            return Ok(base64);
        }
        [HttpDelete("deleteImage")]
        public IActionResult DeleteImage(ImageResizeDto imageResize)
        {
            if (imageResize.Src == null )
            {
                return BadRequest();
            }
            bool imageDeleted = this._imageService.DeleteImage(imageResize.Src, imageResize.Width, imageResize.Height);
            if (imageDeleted == false)
            {
                return NoContent();
            }
            return Ok();
        }
        [HttpDelete("deleteAllImages")]
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