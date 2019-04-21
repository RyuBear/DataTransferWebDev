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

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class SQLSettingController : BaseController
    {
        SQLSettingVM model = new SQLSettingVM();

        [HttpGet]
        public ActionResult Index()
        {
            SQLSettingListVM vm = new SQLSettingListVM();

            using (tblSQLSettingRepository rep = new tblSQLSettingRepository())
            {
                vm.settings = rep.get(vm.SQLName, vm.SQLType);
            }

            ViewBag.SysMsg = TempData["SysMsg"];
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index(SQLSettingListVM vm)
        {
            using (tblSQLSettingRepository rep = new tblSQLSettingRepository())
            {
                vm.settings = rep.get(vm.SQLName, vm.SQLType);
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
            string SQLName = id;

            using (tblSQLSettingRepository rep = new tblSQLSettingRepository())
            {
                tblSQLSetting setting = rep.select(SQLName);
                model.SQLName = setting.SQLName;
                model.SQLStatement = setting.SQLStatement;
                model.SQLType = setting.SQLType;
                model.DataRow = setting.DataRow;
            }
            return View("Edit", model);
        }

        [HttpPost]
        public ActionResult Testing(SQLSettingVM vm)
        {
            if (!Func.SQLIsValid(vm.SQLStatement))
                vm.SQLResult = "SQL語句不合法!";
            else
            {
                using (DataAccess da = new DataAccess())
                {
                    // 將 top(n) 帶入 SQL語句
                    int index = vm.SQLStatement.IndexOf("Select", StringComparison.OrdinalIgnoreCase);  // 找出第一個select的位置
                    if (index < 0)
                    {
                        vm.SQLResult = "SQL語句不合法!";
                    }
                    else
                    {
                        string sql = Func.SqlPlusTop(vm.SQLStatement, vm.DataRow);
                        Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sql);

                        vm.SQLResultDataRow = result.Item2;
                        vm.SQLResult = result.Item3;
                    }
                }
            }
            return View("Edit", vm);
        }

        [HttpPost]
        [ActionName("AjaxIsExist")]
        public ActionResult isExist(string SQLName)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                ModelState.AddModelError("", "非 ajax 呼叫");
                //throw new Exception("非 ajax 呼叫");
            }

            using (tblSQLSettingRepository setting = new tblSQLSettingRepository())
            {
                bool isExist = setting.isExist(SQLName);
                return Json(new { Exist = isExist });
            }
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
                    if (vm.SQLResult.Equals("ok"))
                        return RedirectToAction("Index");
                    else
                        return View("Edit", vm);
                }
            }
        }
        
        public ActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Index");
            }
            using (tblSQLSettingRepository rep = new tblSQLSettingRepository())
            {
                string result = rep.Delete(id);
                if (result != "ok")
                {
                    TempData["SysMsg"] = result;
                }
            }
            return RedirectToAction("Index");
        }
    }
}