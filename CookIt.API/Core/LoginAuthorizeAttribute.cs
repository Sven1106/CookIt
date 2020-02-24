using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CookIt.API.Core
{
    public class LoginAuthorizeAttribute : TypeFilterAttribute
    {
        public LoginAuthorizeAttribute(string claimType, string claimValue) : base(typeof(LoginAuthorizeFilter))
        {
            Arguments = new object[] { new Claim(claimType, claimValue) };
        }
    }

    public class LoginAuthorizeFilter : IAuthorizationFilter
    {
        readonly Claim _claim;

        public LoginAuthorizeFilter(Claim claim)
        {
            _claim = claim;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            bool isAllowAnonymous = context.Filters.Any(item => item is IAllowAnonymousFilter);
            if (!hasClaim && !isAllowAnonymous)
            {
                context.Result = new RedirectToActionResult("Login", "Cms", null);
            }
        }
    }
}
