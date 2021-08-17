using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketData.Model.Request.Adjust;
using MarketData.Models;
using MarketData.Processes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MarketData.Controllers
{
    public class AdjustController : Controller
    {
        private readonly Process process;
        public AdjustController(Process process)
        {
            this.process = process;
        }

        [HttpPost]
        public IActionResult GetAdjustList([FromBody] GetAdjustListRequest request)
        {
            AdjustListViewModel dataModel = new AdjustListViewModel();

            try
            {              
                return Json(dataModel);
            }
            catch (Exception ex)
            {
                return Json(dataModel);
            }
        }
    }
}
