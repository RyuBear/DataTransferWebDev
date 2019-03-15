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
    public class XMLSettingVM
    {
        public string ViewStatus { get; set; }      // 編輯狀態 N:新增  E:編輯
        [Display(Name = "SQL Name")]
        public string SQLName { get; set; }
        
        [Display(Name = "Field View")]
        public List<tblSQLColumns> UnsetColumns { get; set; }      // 未設定之欄位

        public IEnumerable<tblSQLColumns> SetedColumns { get; set; }       // 已設定之欄位

        [Display(Name = "Tag Name")]
        public string TagName { get; set; }


        [Display(Name = "Tag Name")]
        public string newTagName { get; set; }

        [Display(Name = "Field Name")]
        public string FieldName { get; set; }


        [Display(Name = "Default Value")]
        public string DefaultValue { get; set; }

        [Display(Name = "XML Layer")]
        public string FatherTag { get; set; }

        [Display(Name = "Allow Repeat")]
        public bool CanRepeat { get; set; }

        [Display(Name = "XML Name")]
        public string XMLName { get; set; }
        
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "XML Setting Data Row")]
        public List<tblXMLMapping> XMLMappingDataRow { get; set; }

        [Display(Name = "XML View")]
        public string XMLView { get; set; }

        public string SaveResult { get; set; }

        public XMLSettingVM()
        {
            UnsetColumns = new List<tblSQLColumns>();
            SetedColumns = new List<tblSQLColumns>();
            XMLMappingDataRow = new List<tblXMLMapping>();
        }
    }
}
