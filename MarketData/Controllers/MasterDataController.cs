﻿using MarketData.Model.Request;
using MarketData.Model.Request.MasterData;
using MarketData.Model.Response;
using MarketData.Models;
using MarketData.Processes;
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
        private readonly Process process;

        public MasterDataController(Process process)
        {
            this.process = process;
        }


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
        public IActionResult BrandType()
        {
            return View();
        }
        [HttpGet]
        public ActionResult BrandType_Edit(Guid brandTypeID)
        {
            var response = process.masterData.GetBrandTypeDetail(brandTypeID);
            BrandTypeViewModel brandTypeData = new BrandTypeViewModel();

            if (response != null)
            {
                brandTypeData.brandTypeID = response.brandTypeID;
                brandTypeData.brandTypeName = response.brandTypeName;
                brandTypeData.active = response.active;
                brandTypeData.createdDate = response.createdDate;
            }
            return View(brandTypeData);
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


        #region BrandType Function

        [HttpPost]
        public IActionResult GetBrandTypeList()
        {
            BrandTypeListViewModel brandTypeListView = new BrandTypeListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetBrandTypeList();

                if (response != null && response.data != null && response.data.Any())
                {
                    brandTypeListView.brandTypeList = response.data.Select(c => new BrandTypeViewModel
                    {
                        brandTypeID = c.brandTypeID,
                        brandTypeName = c.brandTypeName,
                        active = c.active,
                        createdDate = c.createdDate
                    }).ToList();
                }
                else
                {
                    brandTypeListView.brandTypeList = new List<BrandTypeViewModel>();
                }

                return Json(brandTypeListView);
            }
            else
            {
                return Json(brandTypeListView);
            }
        }

        [HttpGet]
        public IActionResult GetBrandTypeDetail(Guid brandTypeID)
        {
            var response = process.masterData.GetBrandTypeDetail(brandTypeID);
            BrandTypeViewModel brandTypeData = new BrandTypeViewModel();

            if (response != null)
            {
                brandTypeData.brandTypeID = response.brandTypeID;
                brandTypeData.brandTypeName = response.brandTypeName;
                brandTypeData.active = response.active;
                brandTypeData.createdDate = response.createdDate;
            }

            return Json(brandTypeData);
        }

        [HttpPost]
        public IActionResult SaveBrandType(BrandTypeViewModel request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                SaveBrandTypeRequest saveBrandTypeRequest = new SaveBrandTypeRequest
                {
                    brandTypeID = request.brandTypeID == Guid.Empty ? null : request.brandTypeID,
                    active = request.active,
                    brandTypeName = request.brandTypeName
                };

                response = process.masterData.SaveBrandType(saveBrandTypeRequest);
                //return RedirectToAction("BrandType", "MasterData");
                return Json(response);
            }
            else
            {
                response = new SaveDataResponse
                {
                    isSuccess = false
                };
                return Json(response);
            }
        }

        [HttpPost]
        public IActionResult DeleteBrandType([FromBody] DeleteBrandTypeRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                response = process.masterData.DeleteBrandType(request);
                return Json(response);
            }
            else
            {
                response = new SaveDataResponse
                {
                    isSuccess = false
                };
                return Json(response);
            }
        }

        #endregion
    }
}
