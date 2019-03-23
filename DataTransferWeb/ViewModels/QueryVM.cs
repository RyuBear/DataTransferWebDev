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
        public string UserID { get; set; }

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

        #region FTP SET
        [Display(Name = "FTP Server")]
        public string FTPServerIP { get; set; }
        [Display(Name = "Port")]
        public int? FTPPort { get; set; }

        [Display(Name = "User Name")]
        public string FTPUserName { get; set; }
        
        [Display(Name = "Password")]
        public string FTPPassword{ get; set; }
        #endregion

        #region Email SET
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
        #endregion

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
