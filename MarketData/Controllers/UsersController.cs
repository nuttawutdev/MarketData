using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class UsersController : Controller
    {

        public IActionResult Users()
        {
            return View();
        }
        public IActionResult Users_Edit()
        {
            return View();
        }
    }
}
