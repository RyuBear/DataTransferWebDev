using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Transfer.Models;

namespace DataTransferWeb.ViewModels
{
    public class ScheduleListVM
    {
        [Display(Name = "Schedule Name")]
        public string ScheduleName { get; set; }
        
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Format")]
        public string Format { get; set; }

        [Display(Name = "Schedule List")]
        public IEnumerable<tblSchedule> schedules { get; set; }
    }
}
