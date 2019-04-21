
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
    public class CodeMapListVM
    {
        public string UserID { get; set; }


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

        [Display(Name = "Mapping List")]

        public IEnumerable<vwCodeMapping> mapping { get; set; }
    }
}
