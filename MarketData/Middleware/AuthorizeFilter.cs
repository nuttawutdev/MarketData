using MarketData.Model.Response.User;
using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketData.Middleware
{
    public class AuthorizeFilter : ActionFilterAttribute
    {
        private readonly Process _process;
        public AuthorizeFilter(Process process)
        {
            _process = process;
        }

        public async override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var tokenIDSession = filterContext.HttpContext.Session.GetString("tokenID");
            var userDetailSession = filterContext.HttpContext.Session.GetString("userDetail");

            if (tokenIDSession != null && userDetailSession != null)
            {
                var userData = JsonSerializer.Deserialize<GetUserDetailResponse>(userDetailSession);
                var tokenIsValid = _process.user.ValidateToken(tokenIDSession);

                if (!tokenIsValid)
                {
                    var newToken = await _process.user.RefreshToken(userData.userID);
                    filterContext.HttpContext.Session.SetString("tokenID", newToken);

                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(1);
                    option.SameSite = SameSiteMode.Strict;
                    option.IsEssential = true;

                    filterContext.HttpContext.Response.Cookies.Delete("tokenID");
                    filterContext.HttpContext.Response.Cookies.Append("tokenID", newToken, option);
                }
            }
            else
            {
                string tokenIDCookie = filterContext.HttpContext.Request.Cookies["tokenID"];
                string cookieUserData = filterContext.HttpContext.Request.Cookies["userDetail"];

                if (tokenIDCookie != null)
                {
                    var tokenIsValid = _process.user.ValidateToken(tokenIDCookie);

                    if (!tokenIsValid)
                    {
                        filterContext.HttpContext.Session.Clear();
                        filterContext.HttpContext.Response.Cookies.Delete("tokenID");
                        filterContext.HttpContext.Response.Cookies.Delete("userDetail");

                        var values = new RouteValueDictionary(new
                        {
                            action = "Login",
                            controller = "Home",
                        });

                        filterContext.Result = new RedirectToRouteResult(values);
                    }
                    else
                    {
                        filterContext.HttpContext.Session.SetString("userDetail", cookieUserData);
                        filterContext.HttpContext.Session.SetString("tokenID", tokenIDCookie);
                    }
                }
                else
                {
                    filterContext.HttpContext.Session.Clear();
                    filterContext.HttpContext.Response.Cookies.Delete("tokenID");
                    filterContext.HttpContext.Response.Cookies.Delete("userDetail");

                    var values = new RouteValueDictionary(new
                    {
                        action = "Login",
                        controller = "Home",
                    });

                    filterContext.Result = new RedirectToRouteResult(values);
                }
            }
        }
    }
}
