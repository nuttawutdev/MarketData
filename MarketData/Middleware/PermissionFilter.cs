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
using static MarketData.Helper.Utility;

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


            var userDetailSession = filterContext.HttpContext.Session.GetString("userDetail");

            if (userDetailSession != null)
            {
                var userData = JsonSerializer.Deserialize<GetUserDetailResponse>(userDetailSession);

                // ไม่มิสิทธิ์ Key-in 
                if ((actionName == ViewPermission.KeyinByStore.ToString()
                    || actionName == ViewPermission.KeyinByStore_Edit.ToString()
                    || actionName == ViewPermission.KeyinByStore_Edit_View.ToString()) && !userData.keyInData)
                {
                    GoToHome(filterContext);
                }
                else if (userData.approveData && userData.keyInData)
                {
                    if (actionName == ViewPermission.KeyinByStore.ToString() && userData.approveData)
                    {
                        var values = new RouteValueDictionary(new
                        {
                            action = "KeyinByBrand",
                            controller = "KeyIn",
                        });

                        filterContext.Result = new RedirectToRouteResult(values);
                    }
                }
                // ไม่มิสิทธิ์ Key-in by brand
                else if (actionName == ViewPermission.KeyinByBrand.ToString() && !userData.approveData)
                {
                    GoToHome(filterContext);
                }
                else if (controller == ControllerPermission.Users.ToString() && !userData.editUser)
                {
                    GoToHome(filterContext);
                }
                else if (controller == ControllerPermission.MasterData.ToString() && (!userData.editMaster && !userData.viewMaster))
                {
                    GoToHome(filterContext);
                }
                else if (controller == ControllerPermission.Adjust.ToString() && (!userData.approveData && !userData.viewData))
                {
                    GoToHome(filterContext);
                }
                else if (controller == ControllerPermission.Approve.ToString() && (!userData.approveData && !userData.viewData))
                {
                    GoToHome(filterContext);
                }
                else if (controller == ControllerPermission.Reports.ToString() && !userData.viewReport)
                {
                    GoToHome(filterContext);
                }
                else if (actionName == ViewPermission.KeyIn.ToString() && (!userData.keyInData && !userData.approveData))
                {
                    GoToHome(filterContext);
                }
                else if (actionName == ViewPermission.MasterData.ToString() && (!userData.editMaster && !userData.viewMaster))
                {
                    GoToHome(filterContext);
                }
                else if (actionName == ViewPermission.Reports.ToString() && !userData.viewReport)
                {
                    GoToHome(filterContext);
                }
                else if ((actionName == ViewPermission.BrandGroup_Edit.ToString() ||
                    actionName == ViewPermission.BrandSegment_Edit.ToString() ||
                    actionName == ViewPermission.BrandType_Edit.ToString() ||
                    actionName == ViewPermission.Brand_Edit.ToString() ||
                    actionName == ViewPermission.Counter_Edit.ToString() ||
                    actionName == ViewPermission.DepartmentStore_Edit.ToString() ||
                    actionName == ViewPermission.DistributionChannels_Edit.ToString()) && !userData.editMaster)
                {
                    GoToHome(filterContext);
                }
            }
        }

        private void GoToHome(ActionExecutingContext filterContext)
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
