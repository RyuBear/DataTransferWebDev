using Transfer.Models;
using Transfer.Models.Repository;
using Newtonsoft.Json;
using DataTransferWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


public static class DropDownListMenu
{
    /// <summary>
    /// SQL Name 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> SQLNameOption()
    {
        string SQLName = Cache.GetCacheToString("SQLName");
        List<SelectListItem> items = new List<SelectListItem>();
        items.Add(new SelectListItem { Text = "-Please Select-", Value = "", Selected = true });

        using (tblSQLSettingRepository rep = new tblSQLSettingRepository())
        {
            IEnumerable<tblSQLSetting> settings = rep.GetAll();
            foreach (var s in settings)
            {
                if (s.SQLName.Equals(SQLName))
                    items.Add(new SelectListItem { Text = s.SQLName, Value = s.SQLName, Selected = true });
                else
                    items.Add(new SelectListItem { Text = s.SQLName, Value = s.SQLName, Selected = false });
            }
        }
        return items;
    }


    /// <summary>
    /// 依 SQL Namq 取得 欄位
    /// </summary>
    /// <param name="SQLName"></param>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> ColumnsOptionBySQLName(string SQLName)
    {
        List<SelectListItem> items = new List<SelectListItem>();
        items.Add(new SelectListItem { Text = "-Please Select-", Value = "" });
        using (tblSQLColumnsRepository rep = new tblSQLColumnsRepository())
        {
            var columns = rep.getAllColumns(SQLName);
            foreach (var c in columns)
            {
                items.Add(new SelectListItem { Text = c.ColumnName, Value = c.ColumnName });
            }
        }
        return items;
    }


    /// <summary>
    /// Excel 資料型態
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> DataTypeOtion()
    {
        Array values = Enum.GetValues(typeof(DataTypeEnum));
        List<SelectListItem> items = new List<SelectListItem>();
        items.Add(new SelectListItem { Text = "-Please Select-", Value = "" });
        foreach (var c in values)
        {
            items.Add(new SelectListItem { Text = Enum.GetName(typeof(DataTypeEnum), c), Value = Enum.GetName(typeof(DataTypeEnum), c) });
        }
        return items;
    }
}
