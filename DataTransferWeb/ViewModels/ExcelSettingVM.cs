using DataTransferWeb.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;
using Transfer.Models.Models;
using Transfer.Models;
using System.Xml;

namespace DataTransferWeb.ViewModels
{
    public class ExcelSettingVM
    {
        public string ViewStatus { get; set; }      // 編輯狀態 N:新增  E:編輯

        [Display(Name = "SQL Name")]
        public string SQLName { get; set; }
        
        [Display(Name = "Field View")]
        public List<tblSQLColumns> UnsetColumns { get; set; }      // 未設定之欄位

        public IEnumerable<tblSQLColumns> SetedColumns { get; set; }       // 已設定之欄位

        [Display(Name = "Column Name")]
        public string ColumnName { get; set; }

        [Display(Name = "new Column Name")]
        public string newColumnName { get; set; }

        [Display(Name = "Field Name")]
        public string FieldName { get; set; }

        [Display(Name = "Default Value")]
        public string DefaultValue { get; set; }

        [Display(Name = "Data Type")]
        public DataTypeEnum DataType { get; set; }

        [Display(Name = "Sheet")]
        public string SheetName { get; set; }

        [Display(Name = "X")]
        public int X { get; set; }

        [Display(Name = "New Line Char")]
        public string NewLineChar{ get; set; }

        [Display(Name = "Excel Name")]
        public string ExcelName { get; set; }
        
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }


        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "File Name Date Format")]
        public string FileNameDateFormat { get; set; }
        
        [Display(Name = "User ID")]
        public string UserID { get; set; }

        [Display(Name = "Excel Setting Data Row")]
        public List<tblExcelMapping> ExcelMappingDataRow { get; set; }

        public string SaveResult { get; set; }

        public ExcelSettingVM()
        {
            UnsetColumns = new List<tblSQLColumns>();
            SetedColumns = new List<tblSQLColumns>();
            ExcelMappingDataRow = new List<tblExcelMapping>();
        }
    }
}
