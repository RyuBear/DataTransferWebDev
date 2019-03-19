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
    /// SQL Type 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> SQLTypeOption()
    {
        List<SelectListItem> items = new List<SelectListItem>();
        items.Add(new SelectListItem { Text = "-Please Select-", Value = "" });
        items.Add(new SelectListItem() { Text = "提單", Value = "提單" });
        items.Add(new SelectListItem() { Text = "帳單", Value = "帳單" });
        items.Add(new SelectListItem() { Text = "報單", Value = "報單" });
        return items;
    }

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


    /// <summary>
    /// 檔案日期格式 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> DateFormatOption(string selected = "")
    {
        string[] DateFormat = new string[] { "yyyy", "MM", "dd", "hh", "mm", "ss" };

        var items = new List<SelectListItem>();

        var selectedFormat = string.IsNullOrWhiteSpace(selected) ? null : selected.Split(',');

        foreach (var c in DateFormat)
        {
            items.Add(item: new SelectListItem()
            {
                Value = c,
                Text = c,
                Selected = selectedFormat == null
                    ? false
                    : selectedFormat.Contains(c.ToString())
            });
        }
        return items;
    }

    /// <summary>
    /// Format 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> FormatOption(string selected = "")
    {
        string[] Comparisons = new string[] { "XML", "EXCEL" };

        var items = new List<SelectListItem>();
        items.Add(new SelectListItem { Text = "-Please Select-", Value = "" });

        foreach (var c in Comparisons)
        {
            items.Add(item: new SelectListItem()
            {
                Value = c,
                Text = c,
                Selected = (string.IsNullOrEmpty(selected))
                    ? false
                    : selected.Equals(c, StringComparison.OrdinalIgnoreCase)
            });
        }
        return items;
    }

    /// <summary>
    /// Setting 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> SettingOption(string UserID, string CustomerName = "", string Format = "", string selected = "")
    {
        var items = new List<SelectListItem>();
        items.Add(new SelectListItem { Text = "-Please Select-", Value = "" });
        if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(Format))
            return items;

        if (Format.Equals("XML"))
        {
            using (tblXMLSettingRepository rep = new tblXMLSettingRepository())
            {
                List<tblXMLSetting> setting = rep.getByCustomer(UserID, CustomerName).ToList();
                foreach (var s in setting)
                {
                    items.Add(item: new SelectListItem()
                    {
                        Value = s.XMLName,
                        Text = s.XMLName,
                        Selected = (string.IsNullOrEmpty(selected)) ? false : selected.Equals(s.XMLName, StringComparison.OrdinalIgnoreCase)
                    });
                }
            }
        }
        else if (Format.Equals("EXCEL"))
        {
            using (tblExcelSettingRepository rep = new tblExcelSettingRepository())
            {
                List<tblExcelSetting> setting = rep.getByCustomer(UserID, CustomerName).ToList();
                foreach (var s in setting)
                {
                    items.Add(item: new SelectListItem()
                    {
                        Value = s.ExcelName,
                        Text = s.ExcelName,
                        Selected = (string.IsNullOrEmpty(selected)) ? false : selected.Equals(s.ExcelName, StringComparison.OrdinalIgnoreCase)
                    });
                }
            }
        }
        return items;
    }

    /// <summary>
    /// Comparison 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> ComparisonOption(string selected = "")
    {
        string[] Comparisons = new string[] { "=", ">=", "<=", ">", "<" };

        var items = new List<SelectListItem>();

        foreach (var c in Comparisons)
        {
            items.Add(item: new SelectListItem()
            {
                Value = c,
                Text = c,
                Selected = (string.IsNullOrEmpty(selected))
                    ? false
                    : selected.Equals(c, StringComparison.OrdinalIgnoreCase)
            });
        }
        return items;
    }

    /// <summary>
    /// Data Destination 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> DestinationOption(string selected = "")
    {
        string[] Comparisons = new string[] { "Download", "FTP", "EMail" };

        var items = new List<SelectListItem>();
        foreach (var c in Comparisons)
        {
            items.Add(item: new SelectListItem()
            {
                Value = c,
                Text = c,
                Selected = (string.IsNullOrEmpty(selected))
                    ? false
                    : selected.Equals(c, StringComparison.OrdinalIgnoreCase)
            });
        }
        return items;
    }

    /// <summary>
    /// Status 選單
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectListItem> StatusOption(string selected = "")
    {
        string[] Comparisons = new string[] { "成功", "失敗" };

        var items = new List<SelectListItem>();
        items.Add(new SelectListItem { Text = "-Please Select-", Value = "" });
        foreach (var c in Comparisons)
        {
            items.Add(item: new SelectListItem()
            {
                Value = c,
                Text = c,
                Selected = (string.IsNullOrEmpty(selected))
                    ? false
                    : selected.Equals(c, StringComparison.OrdinalIgnoreCase)
            });
        }
        return items;
    }
}
