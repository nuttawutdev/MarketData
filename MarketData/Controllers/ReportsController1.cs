﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class ReportsController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StoreMarketShareZone()
        {
            return View();
        }
        public IActionResult StoreMarketShareValue()
        {
            return View();
        }
        public IActionResult SelectiveMarket()
        {
            return View();
        }
        public IActionResult DetailsSales()
        {
            return View();
        }
        public IActionResult RawData()
        {
            return View();
        }
    }
}
