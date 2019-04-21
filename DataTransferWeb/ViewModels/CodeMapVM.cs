
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
    public class CodeMapVM
    {
        public string UserID { get; set; }
        public string ViewStatus { get; set; }      // 編輯狀態 N:新增  E:編輯

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        
        [Display(Name = "Mode Type")]
        public string ModeType { get; set; }

        [Display(Name = "Format")]
        public string Format { get; set; }

        [Display(Name = "Setting Name")]
        public string SettingName { get; set; }

        [Display(Name = "Tag/Column Name")]
        public string FieldName { get; set; }
        
        [Display(Name = "Before Value")]
        public string BeforeValue { get; set; }
        public string NewBeforeValue { get; set; }

        [Display(Name = "After Value")]
        public string AfterValue { get; set; }

        public string SaveResult { get; set; }

    }
}
