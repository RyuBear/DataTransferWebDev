using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Transfer.Models.Repository;
using Newtonsoft.Json;
using Transfer.Models;
using System.Web.Security;
using DataTransferWeb.Models;
using System.Linq;


namespace DataTransferWeb.ViewModels
{
    public class LoginVM : IValidatableObject
    {
        [Required]
        //[Display(Name = "登入帳號")]
        [Display(Name = "Account")]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Account { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        //[Display(Name = "密碼")]
        [Display(Name = "Password")]
        public string Password { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var guid = Guid.NewGuid().ToString();
            UserData data;

#if (DEBUG)
            if (string.IsNullOrEmpty(Account))
            {
                yield return new ValidationResult("無此使用者", new string[] { "Account" });
            }
            data = new UserData()
            {
                Guid = guid,
                Account = "test",
                Name = "測試者",
                Date = DateTime.Today.Date
            };
            setAuthenticationTicket(data);
            SetManagerInfo(data);
#else
              tblAdminRepository ad = new tblAdminRepository();
            tblAdmin admin = ad.CheckUser(Account, Password);
            if (admin == null)
            {
                yield return new ValidationResult(MessageResource.NoManager, new string[] { "Account" });
            }
            else
            {
                data = new UserData()
                {
                    Guid = guid,
                    Account = admin.PersonalID,
                    Name = admin.NameChi,
                    Date = DateTime.Today.Date
                };
                setAuthenticationTicket(data);
                SetManagerInfo(data);
            }
#endif

        }

        void setAuthenticationTicket(UserData data)
        {
            var userData = new
            {
                account = data.Account,
                name = data.Name,
                date = data.Date,
            };

            string userDataString = JsonConvert.SerializeObject(userData);
            FormsAuthentication.RedirectFromLoginPage(data.Guid, false);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
                   data.Guid,
                   DateTime.Now,
                   DateTime.Now.AddMinutes(120),
                   false,
                   userDataString,
                   FormsAuthentication.FormsCookiePath);
            string encTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
            cookie.HttpOnly = true;
            System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
        }

        private void SetManagerInfo(UserData info)
        {
            Cache.SetLimitedCache(info.Guid + "ManageID", info.Account);
            Cache.SetLimitedCache(info.Guid + "ManageName", info.Name);
            Cache.SetLimitedCache(info.Guid + "Date", info.Date);
        }
    }
}
