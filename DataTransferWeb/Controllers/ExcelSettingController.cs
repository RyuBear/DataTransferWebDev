using DataTransferWeb.Comparer;
using DataTransferWeb.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Transfer.Models;
using Transfer.Models.Repository;

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class ExcelSettingController : BaseController
    {
        /// <summary>
        /// ExcelMappings 暫存 Excel設定資料
        /// ExcelsetedColumns 暫存 已設定之欄位
        /// </summary>
        ExcelSettingVM model = new ExcelSettingVM();

        [HttpGet]
        public ActionResult Index()
        {
            ExcelSettingListVM vm = new ExcelSettingListVM();

            using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
            {
                vm.settings = rep.get(vm.ExcelName, vm.SQLName, vm.CustomerName); ;
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(ExcelSettingListVM vm)
        {

            using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
            {
                vm.settings = rep.get(vm.ExcelName, vm.SQLName, vm.CustomerName);
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult Load(string id)
        {
            Cache.DelCache("ExcelsetedColumns");
            Cache.DelCache("ExcelMappings");

            if (string.IsNullOrEmpty(id))
            {
                return View("Edit", model);
            }

            model.ViewStatus = "E";
            string ExcelName = id;

            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                model.UnsetColumns = rep.getAllColumns(model.SQLName).ToList();
                //var options = new StringBuilder();
                //options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");

                using (tblExcelSettingRepository setRep = new tblExcelSettingRepository())
                using (tblExcelMappingRepository ExcelRep = new tblExcelMappingRepository())
                {
                    tblExcelSetting setting = setRep.get(ExcelName);
                    IEnumerable<tblExcelMapping> mapping = ExcelRep.get(ExcelName);

                    model.ExcelName = setting.ExcelName;
                    model.SQLName = setting.SQLName;
                    model.CustomerName = setting.CustomerName;
                    model.FileName = setting.FileName;
                    model.FileNameDateFormat = setting.FileNameDateFormat;
                    model.UserID = (string.IsNullOrEmpty(setting.UserId)) ? "" : setting.UserId.Substring(1, setting.UserId.Length - 2);

                    foreach (var m in mapping)
                    {
                        model.ExcelMappingDataRow.Add(new tblExcelMapping()
                        {
                            ColumnName = m.ColumnName,
                            FieldName = m.FieldName,
                            DefaultValue = m.DefaultValue,
                            DataType = m.DataType,
                            SheetName = m.SheetName,
                            X = m.X,
                            NewLineChar = m.NewLineChar,
                            CanRepeat = m.CanRepeat
                        });
                    }

                    using (tblSQLColumnsRepository colRep = new tblSQLColumnsRepository())
                    {
                        List<tblSQLColumns> allColumns = colRep.getAllColumns(model.SQLName).ToList();
                        List<tblSQLColumns> ExcelsetedColumns = new List<tblSQLColumns>(); ;
                        foreach (var c in mapping)
                        {
                            ExcelsetedColumns.Add(new tblSQLColumns()
                            {
                                SQLName = model.SQLName,
                                ColumnName = c.FieldName
                            });
                        }
                        model.SetedColumns = ExcelsetedColumns;

                        IEnumerable<tblSQLColumns> unsetColumns = allColumns.Except(ExcelsetedColumns, new ColumnComparer());
                        model.UnsetColumns = unsetColumns.ToList();
                    }
                }
            }

            Cache.SetLimitedCache("ExcelMappings", model.ExcelMappingDataRow);
            Cache.SetLimitedCache("ExcelsetedColumns", model.SetedColumns);
            return View("Edit", model);
        }

        /// <summary>
        /// 取得 Columns 清單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxGetColumns")]
        public ActionResult GetColumns(ExcelSettingVM vm)
        {
            Cache.DelCache("ExcelsetedColumns");
            Cache.DelCache("ExcelMappings");

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");

            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                List<tblSQLColumns> allColumns = rep.getAllColumns(vm.SQLName).ToList();
                foreach (var c in allColumns)
                {
                    options.AppendFormat("<option value='{0}'>{1}</option>", c.ColumnName, c.ColumnName);
                }
            }

            var jsonData = new
            {
                Options = options.ToString()
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 新增 Mapping
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxInsertMapping")]
        public ActionResult InsertMapping(ExcelSettingVM vm)
        {
            List<tblExcelMapping> ExcelMappings = new List<tblExcelMapping>();
            string ColumnName = string.Empty;

            #region 寫入暫存 Session ExcelMappings
            try
            {
                ExcelMappings = JsonConvert.DeserializeObject<List<tblExcelMapping>>(Cache.GetCache("ExcelMappings"));
            }
            catch { }

            // 判斷 Column Name 是否有重複
            if (string.IsNullOrEmpty(vm.ColumnName) || !vm.ColumnName.Equals(vm.newColumnName, StringComparison.OrdinalIgnoreCase))
            {
                tblExcelMapping map = ExcelMappings.Find(x => x.ColumnName.Equals(vm.newColumnName, StringComparison.OrdinalIgnoreCase));
                if (map != null)
                {
                    var result = new
                    {
                        status = "Column Name 已存在！",
                    };
                    return Json(result);
                }
            }
            // 更新時 將舊暫存資料刪除
            tblExcelMapping mapping = ExcelMappings.Find(x => x.ColumnName.Equals(vm.ColumnName, StringComparison.OrdinalIgnoreCase));
            if (mapping != null)
            {
                ColumnName = mapping.FieldName;
                ExcelMappings.Remove(mapping);
            }

            // 將新的設定值寫入
            mapping = new tblExcelMapping()
            {
                ColumnName = vm.newColumnName,
                FieldName = vm.FieldName ?? string.Empty,
                DefaultValue = vm.DefaultValue,
                SheetName = vm.SheetName,
                X = vm.X,
                DataType = vm.DataType.GetDescription(),
                NewLineChar = vm.NewLineChar,
                CanRepeat = vm.CanRepeat
            };
            ExcelMappings.Add(mapping);
            Cache.SetLimitedCache("ExcelMappings", ExcelMappings);
            #endregion

            #region 寫入暫存 Session ExcelsetedColumns
            List<tblSQLColumns> ExcelsetedColumns = new List<tblSQLColumns>(); ;
            try
            {
                ExcelsetedColumns = JsonConvert.DeserializeObject<List<tblSQLColumns>>(Cache.GetCache("ExcelsetedColumns"));
            }
            catch { }

            // 更換指定欄位時
            if (!ColumnName.Equals(vm.FieldName))
            {
                // 更新時 將原指定的欄位刪除
                tblSQLColumns column = ExcelsetedColumns.Find(x => x.ColumnName.Equals(ColumnName, StringComparison.OrdinalIgnoreCase));
                if (column != null)
                    ExcelsetedColumns.Remove(column);

                // 記錄 新指定的欄位
                if (!string.IsNullOrEmpty(vm.FieldName))
                {
                    tblSQLColumns setedColumn = new tblSQLColumns()
                    {
                        SQLName = vm.SQLName,
                        ColumnName = vm.FieldName
                    };
                    ExcelsetedColumns.Add(setedColumn);
                }
            }

            Cache.SetLimitedCache("ExcelsetedColumns", ExcelsetedColumns);
            #endregion

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");
            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                List<tblSQLColumns> allColumns = rep.getAllColumns(vm.SQLName).ToList();
                IEnumerable<tblSQLColumns> unsetColumns = allColumns.Except(ExcelsetedColumns, new ColumnComparer());
                foreach (var c in unsetColumns)
                {
                    options.AppendFormat("<option value='{0}'>{1}</option>", c.ColumnName, c.ColumnName);
                }
            }

            var jsonData = new
            {
                status = "ok",
                Options = options.ToString()
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 刪除 Mapping
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxDeleteMapping")]
        public ActionResult DeleteMapping(ExcelSettingVM vm)
        {
            #region 從暫存 Session ExcelMappings 刪除
            List<tblExcelMapping> ExcelMappings = new List<tblExcelMapping>(); ;
            string ColumnName = string.Empty;

            try
            {
                ExcelMappings = JsonConvert.DeserializeObject<List<tblExcelMapping>>(Cache.GetCache("ExcelMappings"));
            }
            catch { }
            tblExcelMapping mapping = ExcelMappings.Find(x => x.ColumnName.Equals(vm.ColumnName, StringComparison.OrdinalIgnoreCase));
            if (mapping != null)
            {
                ColumnName = mapping.FieldName;
                ExcelMappings.Remove(mapping);
            }
            Cache.SetLimitedCache("ExcelMappings", ExcelMappings);
            #endregion

            #region 從暫存 Session ExcelsetedColumns 刪除
            List<tblSQLColumns> ExcelsetedColumns = new List<tblSQLColumns>(); ;
            try
            {
                ExcelsetedColumns = JsonConvert.DeserializeObject<List<tblSQLColumns>>(Cache.GetCache("ExcelsetedColumns"));
            }
            catch { }
            tblSQLColumns column = ExcelsetedColumns.Find(x => x.ColumnName.Equals(ColumnName, StringComparison.OrdinalIgnoreCase));
            if (column != null)
                ExcelsetedColumns.Remove(column);
            Cache.SetLimitedCache("ExcelsetedColumns", ExcelsetedColumns);
            #endregion

            var options = new StringBuilder();
            options.AppendFormat("<option value='{0}'>{1}</option>", "", "-Please Select-");
            using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
            {
                List<tblSQLColumns> allColumns = rep.getAllColumns(vm.SQLName).ToList();
                IEnumerable<tblSQLColumns> unsetColumns = allColumns.Except(ExcelsetedColumns, new ColumnComparer());
                foreach (var c in unsetColumns)
                {
                    options.AppendFormat("<option value='{0}'>{1}</option>", c.ColumnName, c.ColumnName);
                }
            }

            var jsonData = new
            {
                Options = options.ToString()
            };
            return Json(jsonData);
        }

        /// <summary>
        /// 顯示已設定的Excel設定
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionName("AjaxShowMappingData")]
        [OutputCache(NoStore = true, Duration = 0)] // 以防Server取得的是Cache，必須即時更新
        public ActionResult ShowMappingData(ExcelSettingVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                return Content("Fail");
            }

            #region 讀入暫存 Session ExcelMappings
            List<tblExcelMapping> ExcelMappings = new List<tblExcelMapping>(); ;
            try
            {
                ExcelMappings = JsonConvert.DeserializeObject<List<tblExcelMapping>>(Cache.GetCache("ExcelMappings"));
            }
            catch { }
            #endregion

            model.ExcelMappingDataRow = ExcelMappings.OrderBy(x => x.SheetName).ThenBy(x => x.X).ToList();

            return PartialView("_ExcelMappingRow", model);
        }

        [HttpPost]
        [ActionName("AjaxIsExist")]
        public ActionResult isExist(string ExcelName)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                ModelState.AddModelError("", "非 ajax 呼叫");
                //throw new Exception("非 ajax 呼叫");
            }

            using (tblExcelSettingRepository setting = new tblExcelSettingRepository())
            {
                bool isExist = setting.isExist(ExcelName);
                return Json(new { Exist = isExist });
            }
        }

        [HttpPost]
        public ActionResult Save(ExcelSettingVM vm, string[] DateFormats)
        {
            if (string.IsNullOrEmpty(vm.ExcelName))
                vm.SaveResult += "請輸入 Excel Name!\r\n";
            if (string.IsNullOrEmpty(vm.CustomerName))
                vm.SaveResult += "請輸入 Customer Name!\r\n";
            if (string.IsNullOrEmpty(vm.SQLName))
                vm.SaveResult += "請選擇 SQL Name!\r\n";

            vm.FileNameDateFormat = string.Join(",", DateFormats);

            if (!string.IsNullOrEmpty(vm.SaveResult))
            {
                return View("Edit", vm);
            }
            #region 取得暫存 Session ExcelMappings
            List<tblExcelMapping> ExcelMappings = new List<tblExcelMapping>();
            string ColumnName = string.Empty;

            try
            {
                ExcelMappings = JsonConvert.DeserializeObject<List<tblExcelMapping>>(Cache.GetCache("ExcelMappings"));
            }
            catch (Exception ex) { }
            #endregion

            using (tblExcelSettingRepository setting = new tblExcelSettingRepository())
            {
                vm.SaveResult = setting.Save(vm.ExcelName, vm.CustomerName, vm.SQLName, vm.FileName, vm.FileNameDateFormat, vm.UserID, userInfo.Account, ExcelMappings);
                if (vm.SaveResult.Equals("ok"))
                    return RedirectToAction("Index");
                else
                    return View("Edit", vm);
            }
        }

        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
            {
                rep.Delete(id);
            }
            return RedirectToAction("Index");
        }
    }
}