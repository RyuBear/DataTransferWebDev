﻿using System;
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
    public class QueryController : BaseController
    {
        QueryVM model = new QueryVM();

        [HttpGet]
        public ActionResult Index()
        {
            return View(model);
        }

        /// <summary>
        /// 載入格式名稱
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxLoadFormat")]
        public ActionResult LoadFormat(QueryVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                return Content("Fail");
            }

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");
            if (vm.Format.Equals("XML"))
            {
                using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
                {
                    List<tblXMLSetting> setting = rep.getByCustomer(vm.CustomerName).ToList();
                    foreach (var s in setting)
                    {
                        options.AppendFormat("<option value='{0}'>{1}</option>", s.XMLName, s.XMLName);
                    }
                }
            }

            if (vm.Format.Equals("EXCEL"))
            {
                using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
                {
                    List<tblExcelSetting> setting = rep.getByCustomer(vm.CustomerName).ToList();
                    foreach (var s in setting)
                    {
                        options.AppendFormat("<option value='{0}'>{1}</option>", s.ExcelName, s.ExcelName);
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
        public ActionResult LoadColumns(QueryVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                return Content("Fail");
            }
            string SQLName = string.Empty;
            if (vm.Format.Equals("XML"))
            {
                using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
                {
                    tblXMLSetting setting = rep.get(vm.SettingName);
                    if (setting != null) SQLName = setting.SQLName;
                }
            }

            if (vm.Format.Equals("EXCEL"))
            {
                using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
                {
                    tblExcelSetting setting = rep.get(vm.SettingName);
                    if (setting != null) SQLName = setting.SQLName;
                }
            }

            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                var Columns = rep.getAllColumns(SQLName).ToList();
                vm.Columns = new List<ColumnData>();
                foreach (var c in Columns)
                {
                    vm.Columns.Add(new ColumnData()
                    {
                        ColumnName = c.ColumnName,
                        Idx = c.Idx
                    });
                }
            }
            return PartialView("_ColumnSet", vm);
        }

        [HttpPost]
        public ActionResult Query(QueryVM vm)
        {
            string SQLName = string.Empty;
            if (!canQuery(vm))
            {
                return View("Index", vm);
            }

            #region 取得 SQL Statement
            if (vm.Format.Equals("XML"))
            {
                using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
                {
                    tblXMLSetting setting = rep.get(vm.SettingName);
                    if (setting != null)
                    {
                        SQLName = setting.SQLName;
                        vm.FileName = setting.FileName + ((string.IsNullOrEmpty(setting.FileNameDateFormat)) ? ".xml" : DateTime.Now.ToString(setting.FileNameDateFormat.Replace(",", "")) + ".xml");
                    }
                }
            }
            else if (vm.Format.Equals("EXCEL"))
            {
                using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
                {
                    tblExcelSetting setting = rep.get(vm.SettingName);
                    if (setting != null)
                    {
                        SQLName = setting.SQLName;
                        vm.FileName = setting.FileName + ((string.IsNullOrEmpty(setting.FileNameDateFormat)) ? ".xls" : DateTime.Now.ToString(setting.FileNameDateFormat.Replace(",", "")) + ".xls");
                    }
                }
            }
            #endregion

            using (tblSQLSettingRepository rep = new tblSQLSettingRepository())
            {
                tblSQLSetting setting = rep.select(SQLName);
                using (DataAccess da = new DataAccess())
                {
                    Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(setting.SQLStatement.Replace("\r\n", ""), vm.DataRow, vm.Columns);
                    vm.SQLResultDataRow = result.Item2;
                }
            }
            return View("Index", vm);
        }

        bool canQuery(QueryVM vm)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(vm.CustomerName)) msg += "請輸入 Customer Name\r\n";
            if (string.IsNullOrEmpty(vm.Format)) msg += "請選擇 Format\r\n";
            if (string.IsNullOrEmpty(vm.SettingName)) msg += "請選擇 XML/Excel Name\r\n";

            ViewBag.QueryMsg = msg;
            if (string.IsNullOrEmpty(msg))
                return true;
            else
                return false;
        }

        [HttpPost]
        public ActionResult Generate(QueryVM vm)
        {
            string SQLName = string.Empty;
            if (!canGenerate(vm))
            {
                return View("Index", vm);
            }

            #region XML
            if (vm.Format.Equals("XML"))
            {
                using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
                using (tblXMLMappingRepository map = new tblXMLMappingRepository())
                {
                    tblXMLSetting setting = rep.get(vm.SettingName);
                    if (setting != null)
                    {
                        SQLName = setting.SQLName;
                    }
                    List<tblXMLMapping> mapping = map.get(vm.SettingName).ToList();
                    using (tblSQLSettingRepository set = new tblSQLSettingRepository())
                    {
                        tblSQLSetting sqlSetting = set.select(SQLName);
                        using (DataAccess da = new DataAccess())
                        {
                            Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sqlSetting.SQLStatement.Replace("\r\n", ""), null, vm.Columns);
                            vm.SQLResultDataRow = result.Item2;
                            XmlDocument xmlDoc = Func.GenerateXML(result.Item2, mapping);

                            MemoryStream ms = new MemoryStream();
                            using (XmlWriter writer = XmlWriter.Create(ms))
                            {
                                xmlDoc.WriteTo(writer); // Write to memorystream
                            }

                            byte[] data = ms.ToArray();

                            Response.Clear();
                            Response.ContentType = "application/octet-stream";
                            Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(vm.FileName));
                            Response.Charset = "UTF-8";
                            Response.BinaryWrite(data);
                            Response.End();
                            ms.Flush(); // Probably not needed
                            ms.Close();
                        }
                    }
                }
            }
            #endregion
            #region EXCEL
            else if (vm.Format.Equals("EXCEL"))
            {
                using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
                using (tblExcelMappingRepository map = new tblExcelMappingRepository())
                {
                    tblExcelSetting setting = rep.get(vm.SettingName);
                    if (setting != null)
                    {
                        SQLName = setting.SQLName;
                    }

                    List<tblExcelMapping> mapping = map.get(vm.SettingName).ToList();
                    using (tblSQLSettingRepository set = new tblSQLSettingRepository())
                    {
                        tblSQLSetting sqlSetting = set.select(SQLName);
                        using (DataAccess da = new DataAccess())
                        {
                            Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sqlSetting.SQLStatement.Replace("\r\n", ""), null, vm.Columns);
                            vm.SQLResultDataRow = result.Item2;
                            HSSFWorkbook book = Func.GenerateExcel(result.Item2, mapping);

                            MemoryStream ms = new MemoryStream();
                            book.Write(ms);
                            Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}", HttpUtility.UrlEncode(vm.FileName)));
                            Response.BinaryWrite(ms.ToArray());
                            Response.End();
                            book = null;
                            ms.Close();
                            ms.Dispose();
                        }
                    }
                }
            }
            #endregion

            return View("Index", vm);
        }


        bool canGenerate(QueryVM vm)
        {
            string msg = string.Empty;
            if (string.IsNullOrEmpty(vm.CustomerName)) msg += "請輸入 Customer Name\r\n";
            if (string.IsNullOrEmpty(vm.Format)) msg += "請選擇 Format\r\n";
            if (string.IsNullOrEmpty(vm.SettingName)) msg += "請選擇 XML/Excel Name\r\n";
            if (string.IsNullOrEmpty(vm.DataDestination)) msg += "請選擇 Data Destination\r\n";
            if (string.IsNullOrEmpty(vm.FileName)) msg += "請輸入 File Name\r\n";

            ViewBag.GenerateMsg = msg;
            if (string.IsNullOrEmpty(msg))
                return true;
            else
                return false;
        }

        [HttpPost]
        public ActionResult Save(SQLSettingVM vm)
        {
            if (!Func.SQLIsValid(vm.SQLStatement))
            {
                vm.SQLResult = "SQL語句不合法!";
                return View("Edit", vm);
            }
            else
            {
                using (DataAccess da = new DataAccess())
                {
                    // 將 top(n) 帶入 SQL語句
                    //int index = vm.SQLStatement.IndexOf("Select", StringComparison.OrdinalIgnoreCase);  // 找出第一個select的位置
                    //string sql = "SELECT TOP (" + vm.DataRow + ") " + vm.SQLStatement.Remove(index, 6);
                    string sql = Func.SqlPlusTop(vm.SQLStatement, vm.DataRow);
                    Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sql);

                    vm.SQLResultDataRow = result.Item2;
                    vm.SQLResult = result.Item3;
                }

                List<ColumnData> Columns = new List<ColumnData>();
                for (int i = 0; i < vm.SQLResultDataRow.Columns.Count; i++)
                {
                    Columns.Add(new ColumnData()
                    {
                        ColumnName = vm.SQLResultDataRow.Columns[i].ColumnName,
                        Idx = i
                    });
                }
                using (tblSQLSettingRepository setting = new tblSQLSettingRepository())
                {
                    vm.SQLResult = setting.Save(vm.SQLName, vm.SQLStatement, vm.DataRow, vm.SQLType, Columns, userInfo.Account);
                    if (vm.SQLResult.Equals("ok")) vm.SQLResult = "Save Successful!";
                }
            }
            return RedirectToAction("Index");
        }

    }
}