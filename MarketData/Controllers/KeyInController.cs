using MarketData.Processes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class KeyInController : Controller
    {
        private readonly Process process;

        public KeyInController(Process process)
        {
            this.process = process;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult KeyinByBrand()
        {
            return View();
        }
        public IActionResult KeyinByStore()
        {
            return View();
        }

        public IActionResult KeyinByStore_Edit()
        {
            return View();
        }



    }
}
