
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
    public class ExcelSettingListVM
    {
        [Display(Name = "Excel Name")]
        public string ExcelName { get; set; }
        
        [Display(Name = "SQL Name")]
        public string SQLName { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Excel Setting List")]

        public IEnumerable<tblExcelSetting> settings { get; set; }
    }
}
