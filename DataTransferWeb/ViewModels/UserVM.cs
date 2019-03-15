
using DataTransferWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace DataTransferWeb.ViewModels
{
    public class UserVM
    {
        public UserVM()
        {
            Account = ManagerHelper.Account;
            Name = ManagerHelper.UserName;
        }

        //[Display(Name = "帳號")]
        [Display(Name = "Account")]
        public string Account { get; set; }

        //[Display(Name = "姓名")]
        [Display(Name = "Name")]
        public string Name{ get; set; }
        
        [Required]
        //[Display(Name = "目前使用的密碼")]
        [Display(Name = "CurrentPassword")]
        public string Password { get; set; }

        [Required]
        //[Display(Name = "設定新密碼")]
        [Display(Name = "NewPassword")]
        [Compare("NewPassword_Confirm")]
        public string NewPassword { get; set; }

        [Required]
        //[Display(Name = "再次輸入新密碼")]
        [Display(Name = "NewPassword_Confirm")]
        public string NewPassword_Confirm { get; set; }        
    }
}
