using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Middleware
{
    public class PermissionFilter : ActionFilterAttribute
    {
        private readonly Process _process;
        public PermissionFilter(Process process)
        {
            _process = process;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string controller = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];
            string pageName = filterContext.ActionDescriptor.AttributeRouteInfo?.Template;

            string pathAction = $"{controller}/{actionName}";

            var userDetailSession = filterContext.HttpContext.Session.GetString("userDetail");

            if(userDetailSession != null)
            {

            }
        }
    }
}
