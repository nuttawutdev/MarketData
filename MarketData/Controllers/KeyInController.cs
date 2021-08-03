using MarketData.Model.Response.KeyIn;
using MarketData.Processes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketData.Models;

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

        #region BA KeyIn Function
        //[HttpPost]
        //public IActionResult GetBAKeyInList(Guid userID)
        //{
            //BAKeyInListViewModel BAKeyInListView = new BAKeyInListViewModel();

            //if (ModelState.IsValid)
            //{
            //    var response = process.masterData.GetBrandList();

            //    if (response != null && response.data != null && response.data.Any())
            //    {
                  
            //    }
            //    else
            //    {
            //        BAKeyInListView.brandList = new List<BAKeyInDataViewModel>();
            //    }

            //    return Json(BAKeyInListView);
            //}
            //else
            //{
            //    return Json(BAKeyInListView);
            //}



        //}

        #endregion

    }
}
