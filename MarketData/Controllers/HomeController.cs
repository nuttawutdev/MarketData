using MarketData.Middleware;
using MarketData.Model.Request.User;
using MarketData.Model.Response.User;
using MarketData.Models;
using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class HomeController : Controller
    {
        private readonly Process process;

        public HomeController(Process process)
        {
            this.process = process;
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult MasterData()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Reports()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult KeyIn()
        {
            var userDetailSession = HttpContext.Session.GetString("userDetail");
            var userPermission = JsonSerializer.Deserialize<MarketData.Model.Response.User.GetUserDetailResponse>(userDetailSession);

            if (userPermission.keyInData && !userPermission.approveData)
            {
                return RedirectToAction("KeyinByStore", "KeyIn");
            }
            else if(userPermission.approveData)
            {
                return RedirectToAction("KeyinByBrand", "KeyIn");
            }
            else
            {
                return View("Index");
            }
        }

        [AliveInSystemFilter]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel request)
        {

            if (ModelState.IsValid)
            {
                LoginRequest loginRequest = new LoginRequest
                {
                    userName = request.userName,
                    password = request.password
                };

                var userData = await process.user.Login(loginRequest);

                if (userData != null && userData.userDetail != null)
                {
                    HttpContext.Session.SetString("userDetail", JsonSerializer.Serialize(userData.userDetail));
                    HttpContext.Session.SetString("tokenID", userData.tokenID);

                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddDays(1);
                    option.SameSite = SameSiteMode.Strict;
                    option.IsEssential = true;
                    Response.Cookies.Append("userDetail", JsonSerializer.Serialize(userData.userDetail), option);
                    Response.Cookies.Append("tokenID", userData.tokenID, option);

                    return View("Index");
                }
                else
                {
                    return View("Login");
                }
            }
            else
            {
                return View("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("userDetail"))
                || !string.IsNullOrWhiteSpace(HttpContext.Request.Cookies["userDetail"]))
            {
                GetUserDetailResponse userData;
                string tokenID = string.Empty;

                var sessionData = HttpContext.Session.GetString("userDetail");
                var tokenSession = HttpContext.Session.GetString("tokenID");
                var sessionCookie = HttpContext.Request.Cookies["userDetail"];
                var tokenCookie = HttpContext.Request.Cookies["tokenID"];

                if (sessionData != null)
                {
                    userData = JsonSerializer.Deserialize<GetUserDetailResponse>(sessionData);
                    tokenID = tokenSession;
                }
                else
                {
                    userData = JsonSerializer.Deserialize<GetUserDetailResponse>(sessionCookie);
                    tokenID = tokenCookie;
                }

                var logoutResponse = await process.user.Logout(userData.userID, tokenID);

                if (logoutResponse)
                {
                    Response.Cookies.Delete("userDetail");
                    Response.Cookies.Delete("tokenID");
                    HttpContext.Session.Clear();
                    return RedirectToAction("Login");
                }
                else
                {
                    return View("Index");
                }                          
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
    }
}
