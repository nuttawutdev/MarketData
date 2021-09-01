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
            var sessionData = filterContext.HttpContext.Session.GetString("userDetail");

            if (sessionData != null)
            {
                var userData = JsonSerializer.Deserialize<GetUserDetailResponse>(sessionData);
                //bool validToken = _process.account.ValidateToken(userData.tokenID);
                //if (!validToken)
                //{
                //    var values = new RouteValueDictionary(new
                //    {
                //        action = "Login",
                //        controller = "Account",
                //    });

                //    filterContext.HttpContext.Session.Clear();
                //    filterContext.Result = new RedirectToRouteResult(values);
                //}
            }
            else
            {
                string cookieUserData = filterContext.HttpContext.Request.Cookies["userDetail"];

                if (cookieUserData != null)
                {
                    filterContext.HttpContext.Session.SetString("userDetail", cookieUserData);

                    var userData = JsonSerializer.Deserialize<GetUserDetailResponse>(filterContext.HttpContext.Session.GetString("userDetail"));
                    //bool validToken = _process.account.ValidateToken(userData.tokenID);
                    //if (!validToken)
                    //{
                    //    var values = new RouteValueDictionary(new
                    //    {
                    //        action = "Login",
                    //        controller = "Account",
                    //    });

                    //    filterContext.HttpContext.Session.Clear();
                    //    filterContext.Result = new RedirectToRouteResult(values);
                    //}
                }
                else
                {
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
