using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataTransferWeb.ViewModels;
using Transfer.Models.Repository;
using System.Data;
using Transfer.Models.Models;

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class SQLSettingController : BaseController
    {
        SQLSettingVM model = new SQLSettingVM();

        [HttpGet]
        public ActionResult Index()
        {
            return View(model);
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
                        //string sql = "SELECT TOP (" + vm.DataRow + ") " + vm.SQLStatement.Remove(index, 6);

                        string sql = Func.SQLTop(vm.SQLStatement, vm.DataRow);
                        Tuple<bool, DataTable, string> result = da.TryExecuteDataTable(sql);

                        vm.SQLResultDataRow = result.Item2;
                        vm.SQLResult = result.Item3;
                    }
                }
            }
            return View("Index", vm);
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
                vm.SQLResult = "SQL語句不合法!";
            else
            {
                using (DataAccess da = new DataAccess())
                {
                    // 將 top(n) 帶入 SQL語句
                    //int index = vm.SQLStatement.IndexOf("Select", StringComparison.OrdinalIgnoreCase);  // 找出第一個select的位置
                    //string sql = "SELECT TOP (" + vm.DataRow + ") " + vm.SQLStatement.Remove(index, 6);
                    string sql = Func.SQLTop(vm.SQLStatement, vm.DataRow);
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
            return View("Index", vm);
        }
    }
}