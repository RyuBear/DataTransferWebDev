
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
    public class LogListVM
    {
        [Display(Name = "Date Interval")]
        public string StartDate { get; set; }
        
        [Display(Name = "Date Interval")]
        public string EndDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Log List")]

        public IEnumerable<tblLog> logs { get; set; }
    }
}
