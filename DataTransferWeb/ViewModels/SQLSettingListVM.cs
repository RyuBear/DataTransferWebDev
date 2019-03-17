using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Transfer.Models;

namespace DataTransferWeb.ViewModels
{
    public class SQLSettingListVM
    {
        [Display(Name = "SQL Name")]
        public string SQLName { get; set; }

        [Display(Name = "SQL Type")]
        public string SQLType { get; set; }

        public string SysMsg { get; set; }

        [Display(Name = "SQL Setting List")]
        public IEnumerable<tblSQLSetting> settings { get; set; }
    }
}
