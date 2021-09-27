using MarketData.Middleware;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class ReportsController : Controller
    {
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult Index()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult StoreMarketShareZone()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult StoreMarketShareValue()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult SelectiveMarket()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult DetailsSales()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult RawData()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult SaleByStoreZone()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult SaleByStoreValue()
        {
            return View();
        }
    }
}
