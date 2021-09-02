using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Middleware
{
    public class AliveInSystemFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("tokenID") != null || filterContext.HttpContext.Request.Cookies["tokenID"] != null)
            {
                var values = new RouteValueDictionary(new
                {
                    action = "Index",
                    controller = "Home",
                });

                filterContext.Result = new RedirectToRouteResult(values);
            }
        }
    }
}
