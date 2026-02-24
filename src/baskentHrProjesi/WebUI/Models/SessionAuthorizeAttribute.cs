using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IdentityModel.Tokens.Jwt;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var httpContext = context.HttpContext;
        var token = httpContext.Session.GetString("AccessToken");

        if (string.IsNullOrEmpty(token) || IsTokenExpired(token))
        {
            httpContext.Session.Remove("AccessToken");

            var isAjax = httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                context.Result = new JsonResult(new { success = false, message = "Oturum süreniz doldu." })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
            }
        }
    }

    private bool IsTokenExpired(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadToken(token) as JwtSecurityToken;
            if (jwt == null)
                return true;

            return jwt.ValidTo < DateTime.UtcNow;
        }
        catch
        {
            return true;
        }
    }
}
