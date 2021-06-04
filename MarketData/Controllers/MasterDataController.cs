using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketData.Controllers
{
    public class MasterDataController : Controller
    {
        // GET: MasterDataController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Brand()
        {
            return View();
        }
        public ActionResult Brand_Edit()
        {
            return View();
        }

        public ActionResult BrandGroup()
        {
            return View();
        }
        public ActionResult BrandGroup_Edit()
        {
            return View();
        }
        public ActionResult BrandType()
        {
            return View();
        }

        public ActionResult BrandSegment()
        {
            return View();
        }
        public ActionResult BrandSegment_Edit()
        {
            return View();
        }
        public ActionResult RetailerGroup()
        {
            return View();
        }
        public ActionResult RetailerGroup_Edit()
        {
            return View();
        }
        public ActionResult DistributionChannels()
        {
            return View();
        }
        public ActionResult DistributionChannels_Edit()
        {
            return View();
        }
        public ActionResult DepartmentStore()
        {
            return View();
        }
        public ActionResult DepartmentStore_Edit()
        {
            return View();
        }
        public ActionResult Counter()
        {
            return View();
        }
        public ActionResult Counter_Edit()
        {
            return View();
        }

        // GET: MasterDataController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MasterDataController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MasterDataController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MasterDataController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MasterDataController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MasterDataController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MasterDataController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
