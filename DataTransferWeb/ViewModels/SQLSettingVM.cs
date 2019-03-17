
using DataTransferWeb.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data;

namespace DataTransferWeb.ViewModels
{
    public class SQLSettingVM
    {
        public string ViewStatus { get; set; }

        public List<SelectListItem> TypeItems { get; set; }        // SQL Type選單        

        [Display(Name = "SQL Statement")]
        public string SQLStatement { get; set; }

        [Display(Name = "SQL Result")]
        public string SQLResult { get; set; }

        [Display(Name = "Data Row")]
        public int DataRow { get; set; }


        [Display(Name = "SQL Type")]
        public string SQLType { get; set; }

        [Display(Name = "SQL Name")]
        public string SQLName { get; set; }

        [Display(Name = "SQL Result Data Row")]
        public DataTable SQLResultDataRow { get; set; }

        public SQLSettingVM()
        {
            DataRow = 10;
            SQLResultDataRow = new DataTable();
            TypeItems = new List<SelectListItem>();
            TypeItems.Add(new SelectListItem() { Text = "提單", Value = "提單" });
            TypeItems.Add(new SelectListItem() { Text = "帳單", Value = "帳單" });
            TypeItems.Add(new SelectListItem() { Text = "報單", Value = "報單" });
        }

    }
}
