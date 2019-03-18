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
    public class QueryVM
    {
        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Required]
        [Display(Name = "Format")]
        public string Format { get; set; }

        [Required]
        [Display(Name = "XML/Excel Name")]
        public string SettingName { get; set; }

        [Display(Name = "Field View")]
        public List<ColumnData> Columns { get; set; }      // 欄位清單

        [Display(Name = "Data Row")]
        public int DataRow { get; set; }

        [Display(Name = "SQL Statement")]
        public string SQLStatement { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Data Destination")]
        public string DataDestination { get; set; }
        
        public string DestinationPath { get; set; }

        [Display(Name = "SQL Result Data Row")]
        public DataTable SQLResultDataRow { get; set; }

        public string SaveResult { get; set; }

        public QueryVM()
        {
            Columns = new List<ColumnData>();
            DataRow = 10;
        }
    }
}
