using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataTransferWeb.ViewModels;
using Transfer.Models.Repository;
using System.Data;
using Transfer.Models.Models;
using Transfer.Models;
using System.Text;
using System.IO;
using System.Xml;
using NPOI.HSSF.UserModel;

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class LogController : BaseController
    {
        [HttpGet]
        public ActionResult Index()
        {
            LogListVM vm = new LogListVM();
            using (tblLogRepository rep = new tblLogRepository())
            {
                
                vm.logs = rep.get(vm.StartDate, vm.EndDate, vm.Status);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(LogListVM vm)
        {
            using (tblLogRepository rep = new tblLogRepository())
            {
                vm.logs = rep.get(vm.StartDate, vm.EndDate, vm.Status);
            }
            return View(vm);
        }
    }
}