using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Transfer.Models;

namespace DataTransferWeb.ViewModels
{
    public class ScheduleVM
    {
        public string originScheduleName { get; set; }
        public string ViewStatus { get; set; }
        public string SaveResult { get; set; }
        public string UserID { get; set; }

        [Required]
        [Display(Name = "Schedule Name")]
        public string ScheduleName { get; set; }
        [Required]
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "Format")]
        public string Format { get; set; }
        [Required]
        [Display(Name = "Mode Type")]
        public string ModeType { get; set; }
        [Required]
        [Display(Name = "XML/EXCEL Name")]
        public string SettingName { get; set; }

        [Display(Name = "Destination")]
        public string Destination { get; set; }
        [Display(Name = "Path")]
        public string Path { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "FTP Server")]
        public string FTPServer { get; set; }
        [Display(Name = "Account")]
        public string FTPAccount { get; set; }
        [Display(Name = "Password")]
        public string FTPPassword { get; set; }

        [Required]
        [Display(Name = "Work Type")]
        public string WorkType { get; set; }
        [Display(Name = "Month")]
        public string Month { get; set; }
        [Display(Name = "Date")]
        public string Date { get; set; }
        [Display(Name = "Day")]
        public string Day { get; set; }
        [Display(Name = "Hour")]
        public string Hour { get; set; }
        [Display(Name = "Min")]
        public string Min { get; set; }
        [Display(Name = "Active")]
        public bool Active { get; set; }
        [Display(Name = "Create Date")]
        public DateTime CreateTime { get; set; }
        [Display(Name = "Create By")]
        public string Creator { get; set; }
        [Display(Name = "Update Date")]
        public DateTime? UpdateTime { get; set; }
        [Display(Name = "Update By")]
        public string Updator { get; set; }
    }
}
