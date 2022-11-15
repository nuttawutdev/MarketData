using MarketData.Middleware;
using MarketData.Model.Data;
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

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Brand()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Brand_Edit(Guid brandID)
        {
            var viewData = GetBrandDetail(brandID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Brand_Add(Guid brandID)
        {
            var viewData = GetBrandDetail(brandID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Brand_Edit_View(Guid brandID)
        {
            var viewData = GetBrandDetail(brandID, true);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult BrandGroup()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult BrandGroup_Edit(Guid brandGroupID)
        {
            var viewData = GetBrandGroupDetail(brandGroupID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult BrandGroup_View(Guid brandGroupID)
        {
            var viewData = GetBrandGroupDetail(brandGroupID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public IActionResult BrandType()
        {
            return View();
        }

        [HttpGet]
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
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
        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
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

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult BrandSegment()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult TopDepartmentStore()
        {
            TopDepartmentStoreViewModel dataModel = new TopDepartmentStoreViewModel();

            try
            {
                var reportOption = process.report.GetOptionTopDepartmentStore();
                var topStoreData = process.masterData.GetTopDepartmentStore();

                if (reportOption != null)
                {
                    if (reportOption.departmentStore != null && reportOption.departmentStore.Any())
                    {
                        dataModel.departmentStoreList = reportOption.departmentStore.Select(c => new DepartmentStoreViewModel
                        {
                            departmentStoreID = c.departmentStoreID,
                            departmentStoreName = c.departmentStoreName,
                            retailerGroupName = c.retailerGroupName
                        }).OrderBy(a => a.retailerGroupName).ToList();

                    }
                }

                dataModel.data = topStoreData.data.Select(c => new Models.TopDepartmentStoreData
                {
                    departmentStoreID = c.departmentStoreID,
                    topNumber = c.topNumber
                }).ToList();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }

            return View(dataModel);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult BrandSegment_Edit(Guid brandSegmentID)
        {
            var response = process.masterData.GetBrandSegmentDetail(brandSegmentID);
            BrandSegmentViewModel brandSegmentData = new BrandSegmentViewModel();

            if (response != null)
            {
                brandSegmentData.brandSegmentID = response.brandSegmentID;
                brandSegmentData.brandSegmentName = response.brandSegmentName;
                brandSegmentData.active = response.active;
            }

            return View(brandSegmentData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult BrandSegment_View(Guid brandSegmentID)
        {
            var response = process.masterData.GetBrandSegmentDetail(brandSegmentID);
            BrandSegmentViewModel brandSegmentData = new BrandSegmentViewModel();

            if (response != null)
            {
                brandSegmentData.brandSegmentID = response.brandSegmentID;
                brandSegmentData.brandSegmentName = response.brandSegmentName;
                brandSegmentData.active = response.active;
            }

            return View(brandSegmentData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult RetailerGroup()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult RetailerGroup_Edit(Guid retailerGroupID)
        {
            var viewData = GetRetailerGroupDetail(retailerGroupID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult RetailerGroup_View(Guid retailerGroupID)
        {
            var viewData = GetRetailerGroupDetail(retailerGroupID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult DistributionChannels()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult DistributionChannels_Edit(Guid channelID)
        {
            var viewData = GetChannelDetail(channelID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult DistributionChannels_View(Guid channelID)
        {
            var viewData = GetChannelDetail(channelID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult DepartmentStore()
        {
            return View();
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult DepartmentStore_Edit(Guid departmentStoreID)
        {
            var viewData = GetDepartmentStoreDetail(departmentStoreID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult DepartmentStore_Edit_View(Guid departmentStoreID)
        {
            var viewData = GetDepartmentStoreDetail(departmentStoreID, true);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Counter()
        {
            CounterListViewModel listView = new CounterListViewModel();

            var departmentStoreList = process.masterData.GetDepartmentStoreList();
            var brandList = process.masterData.GetBrandList();
            var channelList = process.masterData.GetDistributionChannelList();
            var retailerGroupList = process.masterData.GetRetailerGroupList();

            listView.departmentStoreList = departmentStoreList != null && departmentStoreList.data != null ? departmentStoreList.data.Where(c => c.active).Select(e => new DepartmentStoreViewModel
            {
                departmentStoreID = e.departmentStoreID,
                departmentStoreName = e.departmentStoreName,
                retailerGroupID = e.retailerGroupID,
                retailerGroupName = e.retailerGroupName,
                distributionChannelID = e.distributionChannelID,
                distributionChannelName = e.distributionChannelName
            }).ToList() : new List<DepartmentStoreViewModel>();
            listView.channelList = channelList != null && channelList.data != null ? channelList.data.Where(c => c.active).Select(e => new DistributionChannelViewModel
            {
                distributionChannelID = e.distributionChannelID,
                distributionChannelName = e.distributionChannelName
            }).ToList() : new List<DistributionChannelViewModel>();
            listView.brandList = brandList != null && brandList.data != null ? brandList.data.Select(e => new BrandViewModel
            {
                brandID = e.brandID,
                brandName = e.brandName
            }).ToList() : new List<BrandViewModel>();
            listView.retailerGroupList = retailerGroupList != null && retailerGroupList.data != null ? retailerGroupList.data.Select(e => new RetailerGroupViewModel
            {
                retailerGroupID = e.retailerGroupID,
                retailerGroupName = e.retailerGroupName
            }).ToList() : new List<RetailerGroupViewModel>();

            return View(listView);

        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Counter_Add(Guid counterID)
        {
            var viewData = GetCounterDetail(counterID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Counter_Edit(Guid counterID)
        {
            var viewData = GetCounterDetail(counterID);
            return View(viewData);
        }

        [ServiceFilter(typeof(AuthorizeFilter))]
        [ServiceFilter(typeof(PermissionFilter))]
        public ActionResult Counter_Edit_View(Guid counterID)
        {
            var viewData = GetCounterDetail(counterID);
            return View(viewData);
        }


        #region Brand Function

        [HttpPost]
        public IActionResult GetBrandList()
        {
            BrandListViewModel brandListView = new BrandListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetBrandList();

                if (response != null && response.data != null && response.data.Any())
                {
                    brandListView.brandList = response.data.Select(c => new BrandViewModel
                    {
                        brandID = c.brandID,
                        brandName = c.brandName,
                        brandGroupName = c.brandGroupName,
                        brandSegmentName = c.brandSegmentName,
                        brandTypeName = c.brandTypeName,
                        universe = c.universe,
                        brandBirth = c.brandBirth,
                        color = c.color,
                        isLorealBrand = c.isLorealBrand,
                        active = c.active
                    }).ToList();
                }
                else
                {
                    brandListView.brandList = new List<BrandViewModel>();
                }

                return Json(brandListView);
            }
            else
            {
                return Json(brandListView);
            }
        }

        public BrandViewModel GetBrandDetail(Guid brandID, bool viewOnly = false)
        {
            var response = process.masterData.GetBrandDetail(brandID);
            BrandViewModel data = new BrandViewModel();
            var brandTypeList = process.masterData.GetBrandTypeList();
            var brandGroupList = process.masterData.GetBrandGroupList();
            var brandSegmentList = process.masterData.GetBrandSegmentList();
            var brandList = process.masterData.GetBrandList();

            var brandTypeSelect = brandTypeList != null && brandTypeList.data != null ? viewOnly ? brandTypeList.data : brandTypeList.data.Where(c => c.active).ToList() : new List<BrandTypeData>();
            var brandGroupSelect = brandGroupList != null && brandGroupList.data != null ? viewOnly ? brandGroupList.data : brandGroupList.data.Where(c => c.active).ToList() : new List<BrandGroupData>();
            var brandSegmentSelect = brandSegmentList != null && brandSegmentList.data != null ? viewOnly ? brandSegmentList.data : brandSegmentList.data.Where(c => c.active).ToList() : new List<BrandSegmentData>();

            var brandSelect = brandList != null && brandList.data != null ? viewOnly ? brandList.data : brandList.data.Where(c => c.active).ToList() : new List<BrandData>();

            data.brandTypeList = brandTypeSelect.Select(e => new BrandTypeViewModel
            {
                brandTypeID = e.brandTypeID,
                brandTypeName = e.brandTypeName
            }).ToList();
            data.brandGroupList = brandGroupSelect.Select(e => new BrandGroupViewModel
            {
                brandGroupID = e.brandGroupID,
                brandGroupName = e.brandGroupName,
                isLoreal = e.isLoreal
            }).ToList();
            data.brandSegmentList = brandSegmentSelect.Select(e => new BrandSegmentViewModel
            {
                brandSegmentID = e.brandSegmentID,
                brandSegmentName = e.brandSegmentName
            }).ToList();
            data.brandList = brandSelect.Select(e => new BrandViewModel
            {
                brandID = e.brandID,
                brandName = e.brandName
            }).ToList();
            

            if (response != null)
            {
                data.brandID = response.brandID;
                data.brandName = response.brandName;
                data.brandShortName = response.brandShortName;
                data.brandGroupID = response.brandGroupID;
                data.brandSegmentID = response.brandSegmentID;
                data.brandTypeID = response.brandTypeID;
                data.color = response.color;
                data.lorealBrandRank = response.lorealBrandRank;
                data.universe = response.universe;
                data.active = response.active;
                data.showInAdjust = response.showInAdjust;
                data.brandInclude = response.brandInclude;
            }

            return data;

        }

        [HttpPost]
        public async Task<IActionResult> SaveBrand([FromBody] SaveBrandRequest request)
        {
            SaveDataResponse response;
            request.brandID = request.brandID == Guid.Empty ? null : request.brandID;
            response = await process.masterData.SaveBrand(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> InsertBrand([FromBody] InsertBrandRequest request)
        {
            SaveDataResponse response;
            request.brandID = request.brandID == Guid.Empty ? null : request.brandID;
            response = await process.masterData.InsertBrand(request);
            return Json(response);
        }

        [HttpPost]
        public IActionResult DeleteBrand([FromBody] DeleteBrandRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                response = process.masterData.DeleteBrand(request);
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
        public async Task<IActionResult> CancelBrand([FromBody] CancelBrandSummaryRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                response = await  process.masterData.CanCelSummaryBrand(request);
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

        [HttpGet]
        public ActionResult DownloadBrandTemplate()
        {
            var path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            string fullPath = path + @"\TemplateImportData\ImportBrand.xlsx";
            byte[] byteArray = System.IO.File.ReadAllBytes(fullPath);
            return File(
                    byteArray,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ImportBrand.xlsx");
        }

        [HttpGet]
        public ActionResult DownloadCounterTemplate()
        {
            var path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            string fullPath = path + @"\TemplateImportData\ImportCounter.xlsx";
            byte[] byteArray = System.IO.File.ReadAllBytes(fullPath);
            return File(
                    byteArray,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ImportCounter.xlsx");
        }

        [HttpGet]
        public ActionResult DownloadStoreTemplate()
        {
            var path = Path.GetDirectoryName(System.AppDomain.CurrentDomain.BaseDirectory);
            string fullPath = path + @"\TemplateImportData\ImportDepartmentStore.xlsx";
            byte[] byteArray = System.IO.File.ReadAllBytes(fullPath);
            return File(
                    byteArray,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"ImportDepartmentStore.xlsx");
        }
        #endregion

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

        [HttpPost]
        public async Task<IActionResult> SaveBrandType([FromBody] SaveBrandTypeRequest request)
        {
            request.brandTypeID = request.brandTypeID == Guid.Empty ? null : request.brandTypeID;
            var response = await process.masterData.SaveBrandType(request);
            return Json(response);
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
                    brandSegmentListView.data = response.data.Select(c => new BrandSegmentViewModel
                    {
                        brandSegmentID = c.brandSegmentID,
                        brandSegmentName = c.brandSegmentName,
                        active = c.active
                    }).ToList();
                }
                else
                {
                    brandSegmentListView.data = new List<BrandSegmentViewModel>();
                }

                return Json(brandSegmentListView);
            }
            else
            {
                return Json(brandSegmentListView);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveBrandSegment([FromBody] SaveBrandSegmentRequest request)
        {
            request.brandSegmentID = request.brandSegmentID == Guid.Empty ? null : request.brandSegmentID;
            var response = await process.masterData.SaveBrandSegment(request);
            return Json(response);
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

        #region Retailer Group Function

        [HttpPost]
        public IActionResult GetRetailerGroupList()
        {
            RetailerGroupListViewModel viewData = new RetailerGroupListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetRetailerGroupList();

                if (response != null && response.data != null && response.data.Any())
                {
                    viewData.data = response.data.Select(c => new RetailerGroupViewModel
                    {
                        retailerGroupID = c.retailerGroupID,
                        retailerGroupName = c.retailerGroupName,
                        active = c.active
                    }).ToList();
                }
                else
                {
                    viewData.data = new List<RetailerGroupViewModel>();
                }

                return Json(viewData);
            }
            else
            {
                return Json(viewData);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveRetailerGroup([FromBody] SaveRetailerGroupRequest request)
        {
            request.retailerGroupID = request.retailerGroupID == Guid.Empty ? null : request.retailerGroupID;
            var response = await process.masterData.SaveRetailerGroup(request);
            return Json(response);
        }

        public RetailerGroupViewModel GetRetailerGroupDetail(Guid retailerGroupID)
        {
            var response = process.masterData.GetRetailerGroupDetail(retailerGroupID);
            RetailerGroupViewModel data = new RetailerGroupViewModel();

            if (response != null)
            {
                data.retailerGroupID = response.retailerGroupID;
                data.retailerGroupName = response.retailerGroupName;
                data.active = response.active;
            }

            return data;

        }

        #endregion

        #region Distribution Channel Function

        [HttpPost]
        public IActionResult GetChannelList()
        {
            DistributionChannelListViewModel viewData = new DistributionChannelListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetDistributionChannelList();

                if (response != null && response.data != null && response.data.Any())
                {
                    viewData.data = response.data.Select(c => new DistributionChannelViewModel
                    {
                        distributionChannelID = c.distributionChannelID,
                        distributionChannelName = c.distributionChannelName,
                        active = c.active
                    }).ToList();
                }
                else
                {
                    viewData.data = new List<DistributionChannelViewModel>();
                }

                return Json(viewData);
            }
            else
            {
                return Json(viewData);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveChannel([FromBody] SaveDistributionChannelRequest request)
        {
            request.distributionChannelID = request.distributionChannelID == Guid.Empty ? null : request.distributionChannelID;
            var response = await process.masterData.SaveDistributionChannel(request);
            return Json(response);
        }

        public DistributionChannelViewModel GetChannelDetail(Guid channelID)
        {
            var response = process.masterData.GetDistributionChannelDetail(channelID);
            DistributionChannelViewModel data = new DistributionChannelViewModel();

            if (response != null)
            {
                data.distributionChannelID = response.distributionChannelID;
                data.distributionChannelName = response.distributionChannelName;
                data.active = response.active;
            }

            return data;

        }

        #endregion

        #region Brand Group Function
        [HttpPost]
        public IActionResult GetBrandGroupList()
        {
            BrandGroupListViewModel viewData = new BrandGroupListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetBrandGroupList();

                if (response != null && response.data != null && response.data.Any())
                {
                    viewData.data = response.data.Select(c => new BrandGroupViewModel
                    {
                        brandGroupID = c.brandGroupID,
                        brandGroupName = c.brandGroupName,
                        isLoreal = c.isLoreal,
                        active = c.active
                    }).ToList();
                }
                else
                {
                    viewData.data = new List<BrandGroupViewModel>();
                }

                return Json(viewData);
            }
            else
            {
                return Json(viewData);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveBrandGroup([FromBody] SaveBrandGroupRequest request)
        {
            request.brandGroupID = request.brandGroupID == Guid.Empty ? null : request.brandGroupID;
            var response = await process.masterData.SaveBrandGroup(request);
            return Json(response);
        }

        public BrandGroupViewModel GetBrandGroupDetail(Guid brandGroupID)
        {
            var response = process.masterData.GetBrandGroupDetail(brandGroupID);
            BrandGroupViewModel data = new BrandGroupViewModel();

            if (response != null)
            {
                data.brandGroupID = response.brandGroupID;
                data.brandGroupName = response.brandGroupName;
                data.isLoreal = response.isLoreal;
                data.active = response.active;
            }

            return data;

        }

        #endregion

        #region DepartmentStore Function

        [HttpPost]
        public IActionResult GetDepartmentStoreList()
        {
            DepartmentStoreListViewModel listView = new DepartmentStoreListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetDepartmentStoreList();

                if (response != null && response.data != null && response.data.Any())
                {
                    listView.data = response.data.Select(c => new DepartmentStoreViewModel
                    {
                        departmentStoreID = c.departmentStoreID,

                        departmentStoreName = c.departmentStoreName,
                        retailerGroupName = c.retailerGroupName,
                        distributionChannelName = c.distributionChannelName,
                        region = c.region,
                        rank = c.rank,
                        active = c.active
                    }).OrderBy(x => x.retailerGroupName).ToList();
                }
                else
                {
                    listView.data = new List<DepartmentStoreViewModel>();
                }

                return Json(listView);
            }
            else
            {
                return Json(listView);
            }
        }

        public DepartmentStoreViewModel GetDepartmentStoreDetail(Guid departmentStoreID, bool viewOnly = false)
        {
            var response = process.masterData.GetDepartmentStoreDetail(departmentStoreID);

            DepartmentStoreViewModel data = new DepartmentStoreViewModel();
            var retailerGroupList = process.masterData.GetRetailerGroupList();
            var channelList = process.masterData.GetDistributionChannelList();
            var regionList = process.masterData.GetRegion();

            var retailerGroupSelect = retailerGroupList != null && retailerGroupList.data != null ? viewOnly ? retailerGroupList.data : retailerGroupList.data.Where(c => c.active).ToList() : new List<RetailerGroupData>();
            var channelSelect = channelList != null && channelList.data != null ? viewOnly ? channelList.data : channelList.data.Where(c => c.active).ToList() : new List<DistributionChannelData>();

            data.retailerGroupList = retailerGroupSelect.Select(e => new RetailerGroupViewModel
            {
                retailerGroupID = e.retailerGroupID,
                retailerGroupName = e.retailerGroupName
            }).ToList();
            data.channelList = channelSelect.Select(e => new DistributionChannelViewModel
            {
                distributionChannelID = e.distributionChannelID,
                distributionChannelName = e.distributionChannelName
            }).ToList();
            data.regionList = regionList != null && regionList.data != null ? regionList.data.Select(e => new RegionViewModel
            {
                regionID = e.regionID,
                regionName = e.regionName
            }).ToList() : new List<RegionViewModel>();

            if (response != null)
            {
                data.departmentStoreID = response.departmentStoreID;
                data.departmentStoreName = response.departmentStoreName;
                data.retailerGroupID = response.retailerGroupID;
                data.regionID = response.regionID;
                data.distributionChannelID = response.distributionChannelID;
                data.rank = response.rank;
                data.active = response.active;
            }

            return data;

        }

        [HttpPost]
        public async Task<IActionResult> SaveDepartmentStore([FromBody] SaveDepsrtmentStoreRequest request)
        {
            request.departmentStoreID = request.departmentStoreID == Guid.Empty ? null : request.departmentStoreID;
            var response = await process.masterData.SaveDepartmentStore(request);
            return Json(response);
        }

        #endregion

        #region Counter Function

        [HttpPost]
        public IActionResult GetCounterList()
        {
            CounterListViewModel listView = new CounterListViewModel();

            if (ModelState.IsValid)
            {
                var response = process.masterData.GetCounterList();

                if (response != null && response.data != null && response.data.Any())
                {
                    listView.data = response.data.Select(c => new CounterViewModel
                    {
                        counterID = c.counterID,
                        retailerGroupID = c.retailerGroupID,
                        retailerGroupName = c.retailerGroupName,
                        distributionChannelID = c.distributionChannelID,
                        distributionChannelName = c.distributionChannelName,
                        departmentStoreID = c.departmentStoreID,
                        departmentStoreName = c.departmentStoreName,
                        brandID = c.brandID,
                        brandName = c.brandName,
                        active = c.active
                    }).ToList();
                }
                else
                {
                    listView.data = new List<CounterViewModel>();
                }

                return Json(listView);
            }
            else
            {
                return Json(listView);
            }
        }

        public CounterViewModel GetCounterDetail(Guid counterID, bool viewOnly = false)
        {
            var response = process.masterData.GetCounterDetail(counterID);

            CounterViewModel data = new CounterViewModel();
            var departmentStoreList = process.masterData.GetDepartmentStoreList();
            var brandList = process.masterData.GetBrandList();
            var channelList = process.masterData.GetDistributionChannelList();

            var departmentSelect = departmentStoreList != null && departmentStoreList.data != null ? viewOnly ? departmentStoreList.data : departmentStoreList.data.Where(c => c.active).ToList() : new List<DepartmentStoreData>();
            var brandSelect = brandList != null && brandList.data != null ? viewOnly ? brandList.data : brandList.data.Where(c => c.active).ToList() : new List<BrandData>();
            var channelSelect = channelList != null && channelList.data != null ? viewOnly ? channelList.data : channelList.data.Where(c => c.active).ToList() : new List<DistributionChannelData>();

            data.departmentStoreList = departmentSelect.Select(e => new DepartmentStoreViewModel
            {
                departmentStoreID = e.departmentStoreID,
                departmentStoreName = e.departmentStoreName,
                distributionChannelID = e.distributionChannelID,
                distributionChannelName = e.distributionChannelName
            }).ToList();
            data.channelList = channelSelect.Select(e => new DistributionChannelViewModel
            {
                distributionChannelID = e.distributionChannelID,
                distributionChannelName = e.distributionChannelName
            }).ToList();
            data.brandList = brandSelect.Select(e => new BrandViewModel
            {
                brandID = e.brandID,
                brandName = e.brandName
            }).ToList();

            if (response != null)
            {
                data.departmentStoreID = response.departmentStoreID;
                data.departmentStoreName = response.departmentStoreName;
                data.counterID = response.counterID;
                data.brandID = response.brandID;
                data.brandName = response.brandName;
                data.distributionChannelID = response.distributionChannelID;
                data.distributionChannelName = response.distributionChannelName;
                data.active = response.active;
                data.alwayShow = response.alwayShow;
            }

            return data;

        }

        [HttpPost]
        public async Task<IActionResult> SaveCounter([FromBody] SaveCounterRequest request)
        {
            request.counterID = request.counterID == Guid.Empty ? null : request.counterID;
            var response = await process.masterData.SaveCounter(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> EditCounter([FromBody] EditCounterRequest request)
        {
            request.counterID = request.counterID == Guid.Empty ? null : request.counterID;
            var response = await process.masterData.EditCounter(request);
            return Json(response);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCounter([FromBody] DeleteCounterRequest request)
        {
            SaveDataResponse response;

            if (ModelState.IsValid)
            {
                response = await process.masterData.DeleteCounter(request);
                return Json(response);
            }
            else
            {
                response = new SaveDataResponse
                {
                    isSuccess = false,
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

        [HttpPost]
        public async Task<IActionResult> SaveTopDepartmentStore([FromBody] SaveTopDepartmentRequest request)
        {
            var response = await process.masterData.SaveTopSepartmentStore(request);
            return Json(response);
        }
    }
}
