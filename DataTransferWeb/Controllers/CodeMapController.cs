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
    public class CodeMapController : BaseController
    {
        CodeMapVM model = new CodeMapVM();

        [HttpGet]
        public ActionResult Index()
        {
            CodeMapListVM vm = new CodeMapListVM();
            using (vwCodeMappingRepository rep = new vwCodeMappingRepository())
            {
                vm.mapping = rep.query(vm.CustomerName, vm.ModeType, vm.Format, vm.SettingName, vm.FieldName);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(CodeMapListVM vm)
        {
            using (vwCodeMappingRepository rep = new vwCodeMappingRepository())
            {
                vm.mapping = rep.query(vm.CustomerName, vm.ModeType, vm.Format, vm.SettingName, vm.FieldName);
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult Load(string ModeType, string Format, string SettingName, string FieldName, string BeforeValue)
        {
            model.UserID = userInfo.Account;

            if (string.IsNullOrEmpty(ModeType)
             || string.IsNullOrEmpty(Format)
             || string.IsNullOrEmpty(SettingName)
             || string.IsNullOrEmpty(FieldName)
             || string.IsNullOrEmpty(BeforeValue))
            {
                return View("New", model);
            }

            model.ViewStatus = "E";
            using (vwCodeMappingRepository rep = new vwCodeMappingRepository())
            {
                vwCodeMapping map = rep.get(SettingName, ModeType, Format, FieldName, BeforeValue);
                model.CustomerName = map.CustomerName;
                model.ModeType = map.ModeType;
                model.Format = map.Format;
                model.SettingName = map.SettingName;
                model.FieldName = map.FieldName;
                model.BeforeValue = map.BeforeValue;
                model.AfterValue = map.AfterValue;
            }
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Save(CodeMapVM vm)
        {
            if (string.IsNullOrEmpty(vm.CustomerName)) vm.SaveResult += "請輸入 Customer Name!\r\n";
            if (string.IsNullOrEmpty(vm.ModeType)) vm.SaveResult += "請選擇 Mode Type!\r\n";
            if (string.IsNullOrEmpty(vm.Format)) vm.SaveResult += "請選擇 Format!\r\n";
            if (string.IsNullOrEmpty(vm.SettingName)) vm.SaveResult += "請選擇 XML/EXCEL Name!\r\n";
            if (string.IsNullOrEmpty(vm.FieldName)) vm.SaveResult += "請選擇 Tag/Column Name!\r\n";

            if (!string.IsNullOrEmpty(vm.SaveResult))
            {
                if (vm.ViewStatus == "E")
                    return View("Edit", vm);
                else
                    return View("New", vm);
            }
            using (tblCodeMappingRepository rep = new tblCodeMappingRepository())
            {
                if (vm.ViewStatus == "E")  // 更新
                    vm.SaveResult = rep.UpadteMapping(vm.SettingName, vm.Format, vm.ModeType, vm.FieldName, vm.BeforeValue, vm.NewBeforeValue, vm.AfterValue, userInfo.Account);
                else
                    vm.SaveResult = rep.InsertMapping(vm.SettingName, vm.Format, vm.ModeType, vm.FieldName, vm.NewBeforeValue, vm.AfterValue, userInfo.Account);
                if (vm.SaveResult.Equals("ok"))
                    return RedirectToAction("Index");
                else
                {
                    if (vm.ViewStatus == "E")
                        return View("Edit", vm);
                    else
                        return View("New", vm);
                }
            }

        }

        [HttpGet]
        public ActionResult Delete(string ModeType, string Format, string SettingName, string FieldName, string BeforeValue)
        {
            if (string.IsNullOrEmpty(ModeType)
             || string.IsNullOrEmpty(Format)
             || string.IsNullOrEmpty(SettingName)
             || string.IsNullOrEmpty(FieldName)
             || string.IsNullOrEmpty(BeforeValue))
            {
                return RedirectToAction("Index");
            }
            using (tblCodeMappingRepository rep = new tblCodeMappingRepository())
            {
                rep.DeleteMapping(SettingName, Format, ModeType, FieldName, BeforeValue);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 載入格式名稱
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxLoadFormat")]
        public ActionResult LoadFormat(CodeMapVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                return Content("Fail");
            }

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");

            if (vm.ModeType.Equals("EXPORT", StringComparison.OrdinalIgnoreCase))
            {
                if (vm.Format.Equals("XML"))
                {
                    using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
                    {
                        List<tblXMLSetting> setting = rep.getByCustomer(userInfo.Account, vm.CustomerName).ToList();
                        foreach (var s in setting)
                        {
                            options.AppendFormat("<option value='{0}'>{1}</option>", s.XMLName, s.XMLName);
                        }
                    }
                }
                else if (vm.Format.Equals("EXCEL"))
                {
                    using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
                    {
                        List<tblExcelSetting> setting = rep.getByCustomer(userInfo.Account, vm.CustomerName).ToList();
                        foreach (var s in setting)
                        {
                            options.AppendFormat("<option value='{0}'>{1}</option>", s.ExcelName, s.ExcelName);
                        }
                    }
                }
            }
            var jsonData = new
            {
                status = "ok",
                Options = options.ToString(),
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 載入欄位
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxLoadColumns")]
        public ActionResult LoadColumns(CodeMapVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                return Content("Fail");
            }
            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");

            string SQLName = string.Empty;
            if (vm.ModeType.Equals("EXPORT", StringComparison.OrdinalIgnoreCase))
            {
                if (vm.Format.Equals("XML"))
                {
                    using (tblXMLMappingRepository rep = new tblXMLMappingRepository())
                    {
                        IEnumerable<tblXMLMapping> mapping = rep.get(vm.SettingName);
                        foreach (var m in mapping)
                        {
                            options.AppendFormat("<option value='{0}'>{0}</option>", m.TagName);
                        }
                    }
                }
                else if (vm.Format.Equals("EXCEL"))
                {
                    using (tblExcelMappingRepository rep = new tblExcelMappingRepository())
                    {
                        IEnumerable<tblExcelMapping> mapping = rep.get(vm.SettingName);
                        foreach (var m in mapping)
                        {
                            options.AppendFormat("<option value='{0}'>{0}</option>", m.ColumnName);
                        }
                    }
                }
            }
            var jsonData = new
            {
                status = "ok",
                Options = options.ToString(),
            };
            return Json(jsonData);
        }
    }
}