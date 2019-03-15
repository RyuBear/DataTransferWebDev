
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferWeb.Models
{
    public class UserData
    {
        public string Guid { get; set; }

        //[Display(Name = "帳號")]
        [Display(Name = "Account")]
        public string Account { get; set; }

        //[Display(Name = "姓名")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        //[Display(Name = "日期")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
