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

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var tokenIDSession = filterContext.HttpContext.Session.GetString("tokenID");
            var userDetailSession = filterContext.HttpContext.Session.GetString("userDetail");

            if (tokenIDSession != null && userDetailSession != null)
            {
                var userData = JsonSerializer.Deserialize<GetUserDetailResponse>(userDetailSession);
                var tokenValidate = _process.user.ValidateToken(tokenIDSession);

                if (!tokenValidate.isValid)
                {
                    if (tokenValidate.tokenExpire)
                    {
                        var newToken = _process.user.RefreshToken(userData.userID);
                        filterContext.HttpContext.Session.SetString("tokenID", newToken);

                        CookieOptions option = new CookieOptions();
                        option.Expires = DateTime.Now.AddDays(1);
                        option.SameSite = SameSiteMode.Strict;
                        option.IsEssential = true;

                        filterContext.HttpContext.Response.Cookies.Delete("tokenID");
                        filterContext.HttpContext.Response.Cookies.Append("tokenID", newToken, option);
                    }
                    else if (tokenValidate.notExistToken)
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
            else
            {
                string tokenIDCookie = filterContext.HttpContext.Request.Cookies["tokenID"];
                string cookieUserData = filterContext.HttpContext.Request.Cookies["userDetail"];

                if (tokenIDCookie != null)
                {
                    var tokenValidate = _process.user.ValidateToken(tokenIDCookie);

                    if (!tokenValidate.isValid)
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
