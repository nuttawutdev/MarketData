using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class ApproveController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
