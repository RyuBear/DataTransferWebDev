using DataTransferWeb.ViewModels;
using System;
using System.Linq;
using System.Web.Mvc;
using Transfer.Models;
using Transfer.Models.Repository;

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class ScheduleController : BaseController
    {
        ScheduleVM model = new ScheduleVM();
        public ActionResult Index()
        {
            ScheduleListVM vm = new ScheduleListVM();
            using (tblScheduleRepository rep = new tblScheduleRepository())
            {
                vm.schedules = rep.get(vm.ScheduleName, vm.CustomerName, vm.Format);
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult Load(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return View("Edit", model);
            }

            model.ViewStatus = "E";
            model.originScheduleName = id;
            model.UserID = userInfo.Account;

            using (tblScheduleRepository rep = new tblScheduleRepository())
            {
                tblSchedule s = rep.get(id);
                model.ScheduleName = s.ScheduleName;
                model.ModeType = s.ModeType;
                model.Format = s.Format;
                model.CustomerName = s.CustomerName;
                model.SettingName = s.SettingName;
                model.Destination = s.Destination;
                model.Path = s.Path;
                model.Email = s.Email;
                model.FTPServer = s.FTPServer;
                model.FTPAccount = s.FTPAccount;
                model.FTPPassword = s.FTPPassword;
                model.WorkType = s.WorkType;
                model.Month = s.Month;
                model.Date = s.Date;
                model.Day = s.Day;
                model.Hour = s.Hour;
                model.Min = s.Min;
                model.Active = s.Active;
            }

            return View("Edit", model);
        }

        [HttpPost]
        [ActionName("AjaxIsExist")]
        public ActionResult isExist(string Name)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                ModelState.AddModelError("", "非 ajax 呼叫");
                //throw new Exception("非 ajax 呼叫");
            }

            using (tblScheduleRepository rep = new tblScheduleRepository())
            {
                bool isExist = rep.isExist(Name);
                return Json(new { Exist = isExist });
            }
        }

        [HttpPost]
        public ActionResult Save(ScheduleVM vm, string[] Destinations)
        {
            if (string.IsNullOrEmpty(vm.ScheduleName))
                vm.SaveResult += "請輸入 Schedule Name!<br />";
            if (string.IsNullOrEmpty(vm.ModeType))
                vm.SaveResult += "請選擇 Mode Type!<br />";
            if (string.IsNullOrEmpty(vm.CustomerName))
                vm.SaveResult += "請輸入 Customer Name!<br />";
            if (string.IsNullOrEmpty(vm.Format))
                vm.SaveResult += "請選擇 Format!<br />";
            if (string.IsNullOrEmpty(vm.SettingName))
                vm.SaveResult += "請選擇 Setting Name!<br />";
            if (string.IsNullOrEmpty(vm.WorkType))
                vm.SaveResult += "請選擇 Work Type!<br />";

            // DATE
            if (vm.WorkType.Equals("1"))
            {
                if (string.IsNullOrEmpty(vm.Date)) vm.SaveResult += "請輸入 Date!<br />";
                // check Month
                if (!string.IsNullOrEmpty(vm.Month) && !Func.IsNumeric(vm.Month, ",", 1, 12)) vm.SaveResult += "請檢查 Month 是否正確(超出範圍 1-12)!<br />";
                // check Date
                if (!string.IsNullOrEmpty(vm.Date) && !Func.IsNumeric(vm.Date, ",", 1, 31)) vm.SaveResult += "請檢查 Date 是否正確(超出範圍 1-31)!<br />";
            }
            // DAY
            else if (vm.WorkType.Equals("2"))
            {
                if (string.IsNullOrEmpty(vm.Min)) vm.SaveResult += "請輸入 Min!<br />";
                // check Day
                if (!string.IsNullOrEmpty(vm.Day) && !Func.IsNumeric(vm.Day, ",", 1, 7)) vm.SaveResult += "請檢查 Day 是否正確(超出範圍 1-7)!<br />";
                // check Hour
                if (!string.IsNullOrEmpty(vm.Hour) && !Func.IsNumeric(vm.Hour, ",", 0, 23)) vm.SaveResult += "請檢查 Hour 是否正確(超出範圍 0-23)!<br />";
                // check Min
                if (!string.IsNullOrEmpty(vm.Min) && !Func.IsNumeric(vm.Min, ",", 0, 59)) vm.SaveResult += "請檢查 Min 是否正確(超出範圍 0-59)!<br />";
            }

            if (Destinations == null || Destinations.Length == 0)
            {
                if (vm.ModeType.Equals("EXPORT"))
                    vm.SaveResult += "請至少選擇一項 Destination!<br />";
                else if (vm.ModeType.Equals("IMPORT"))
                    vm.SaveResult += "請選擇 Source!<br />";
            }
            else
            {
                // Path
                if (Destinations.Contains("1") && string.IsNullOrEmpty(vm.Path)) vm.SaveResult += "請輸入 Path!<br />";
                // Email
                if (Destinations.Contains("2") && string.IsNullOrEmpty(vm.Email)) vm.SaveResult += "請輸入 EMail!<BR />";
                // FTP
                if (Destinations.Contains("3"))
                {
                    if (string.IsNullOrEmpty(vm.FTPServer)) vm.SaveResult += "請輸入 FTP Server!<br />";
                    if (string.IsNullOrEmpty(vm.FTPAccount)) vm.SaveResult += "請輸入 FTP Account!<br />";
                    if (string.IsNullOrEmpty(vm.FTPPassword)) vm.SaveResult += "請輸入 FTP Password!<br />";
                }
            }
            if (!string.IsNullOrEmpty(vm.SaveResult))
            {
                return View("Edit", vm);
            }

            using (tblScheduleRepository rep = new tblScheduleRepository())
            {
                tblSchedule s = new tblSchedule()
                {
                    ScheduleName = vm.ScheduleName,
                    CustomerName = vm.CustomerName,
                    ModeType = vm.ModeType,
                    Format = vm.Format,
                    SettingName = vm.SettingName,
                    Destination = string.Join(",", Destinations),
                    WorkType = vm.WorkType,
                    Active = vm.Active
                };
                // DATE
                if (vm.WorkType.Equals("1"))
                {
                    s.Month = vm.Month;
                    s.Date = vm.Date;
                }
                // DAY
                else if (vm.WorkType.Equals("2"))
                {
                    s.Day = vm.Day;
                    s.Hour = vm.Hour;
                    s.Min = vm.Min;
                }

                // Destination-PATH
                if (Destinations.Contains("1"))
                    s.Path = vm.Path;
                // Destination-EMAIL
                if (Destinations.Contains("2"))
                    s.Email = vm.Email;
                // Destination-FTP
                if (Destinations.Contains("3"))
                {
                    s.FTPAccount = vm.FTPAccount;
                    s.FTPPassword = vm.FTPPassword;
                    s.FTPServer = vm.FTPServer;
                }

                vm.SaveResult = rep.Save(vm.originScheduleName, s, userInfo.Account);
                if (vm.SaveResult.Equals("ok")) vm.SaveResult = "Save Successful!";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            using (tblScheduleRepository rep = new tblScheduleRepository())
            {
                rep.Delete(id);
            }
            return RedirectToAction("Index");
        }
    }
}