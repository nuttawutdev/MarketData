using MarketData.Model.Request;
using MarketData.Model.Request.MasterData;
using MarketData.Model.Response;
using MarketData.Model.Response.MasterData;
using MarketData.Models;
using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
        [HttpGet]
        public ActionResult BrandType_Edit_View(Guid brandTypeID)
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
        public async Task<IActionResult> SaveBrandType([FromBody] SaveBrandTypeRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                request.brandTypeID = request.brandTypeID == Guid.Empty ? null : request.brandTypeID;
                response = await process.masterData.SaveBrandType(request);
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

        #region Brand Segment Function

        [HttpPost]
        public IActionResult GetBrandSegmentList()
        {
            BrandSegmentListViewModel brandSegmentListView = new BrandSegmentListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetBrandSegmentList();

                if (response != null && response.data != null && response.data.Any())
                {
                    brandSegmentListView.brandSegmentList = response.data.Select(c => new BrandSegmentViewModel
                    {
                        brandSegmentID = c.brandSegmentID,
                        brandSegmentName = c.brandSegmentName,
                        active = c.active,
                        createdDate = c.createdDate
                    }).ToList();
                }
                else
                {
                    brandSegmentListView.brandSegmentList = new List<BrandSegmentViewModel>();
                }

                return Json(brandSegmentListView);
            }
            else
            {
                return Json(brandSegmentListView);
            }
        }

        [HttpGet]
        public IActionResult GetBrandSegmentDetail(Guid brandSegmentID)
        {
            var response = process.masterData.GetBrandSegmentDetail(brandSegmentID);
            BrandSegmentViewModel brandSegmentData = new BrandSegmentViewModel();

            if (response != null)
            {
                brandSegmentData.brandSegmentID = response.brandSegmentID;
                brandSegmentData.brandSegmentName = response.brandSegmentName;
                brandSegmentData.active = response.active;
                brandSegmentData.createdDate = response.createdDate;
            }

            return Json(brandSegmentData);
        }

        [HttpPost]
        public async Task<IActionResult> SaveBrandSegment([FromBody] SaveBrandSegmentRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                request.brandSegmentID = request.brandSegmentID == Guid.Empty ? null : request.brandSegmentID;
                response = await process.masterData.SaveBrandSegment(request);
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
        public IActionResult DeleteBrandSegment([FromBody] DeleteBrandSegmentRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                response = process.masterData.DeleteBrandSegment(request);
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

        [HttpPost]
        public async Task<IActionResult> ImportBrand(string userID, IFormFile excelFile)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ImportDataResponse response = new ImportDataResponse();

            using (var stream = new MemoryStream())
            {
                excelFile.CopyTo(stream);
                stream.Position = 0;

                ImportDataRequest request = new ImportDataRequest
                {
                    fileStream = stream,
                    filePath = excelFile.FileName,
                    userID = userID,
                };

                response = await process.masterData.ImportBrandData(request);
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> ImportDepartmentStore(string userID, IFormFile excelFile)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ImportDataResponse response = new ImportDataResponse();

            using (var stream = new MemoryStream())
            {
                excelFile.CopyTo(stream);
                stream.Position = 0;

                ImportDataRequest request = new ImportDataRequest
                {
                    fileStream = stream,
                    filePath = excelFile.FileName,
                    userID = userID,
                };

                response = await process.masterData.ImportDepartmentStoreData(request);
            }

            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> ImportCounter(string userID, IFormFile excelFile)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ImportDataResponse response = new ImportDataResponse();

            using (var stream = new MemoryStream())
            {
                excelFile.CopyTo(stream);
                stream.Position = 0;

                ImportDataRequest request = new ImportDataRequest
                {
                    fileStream = stream,
                    filePath = excelFile.FileName,
                    userID = userID,
                };

                response = await process.masterData.ImportCounterData(request);
            }

            return Json(response);
        }
    }
}
