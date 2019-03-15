using DataTransferWeb.ViewModels;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public static DateTime Now
        {
            get
            {
                DateTime localtime = DateTime.UtcNow;
                TimeZoneInfo TW_TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
                DateTime TW_DateTime = TimeZoneInfo.ConvertTime(localtime, TW_TimeZoneInfo);
                return TW_DateTime;
            }
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            Cache.DelAllCache();
            FormsAuthentication.SignOut();

            LoginVM vm = new LoginVM();
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginVM vm)
        {
            //沒通過Model驗證(必填欄位沒填，DB無此帳密)
            if (!ModelState.IsValid)
            {
                return View("Login", vm);
            }
            else
            {
                return RedirectToAction("Index", "SQLSetting");
            }
        }

        public ActionResult Logout()
        {
            Cache.DelAllCache();
            FormsAuthentication.SignOut();
            LoginVM vm = new LoginVM();
            return View("Login", vm);
        }


        [AllowAnonymous]
        public ActionResult SetCulture(string culture, string returnUrl)
        {
            // Validate input 
            culture = CultureHelper.GetImplementedCulture(culture);

            // Save culture in a cookie 
            HttpCookie cookie = Request.Cookies["_culture"];

            if (cookie != null)
            {
                // update cookie value 
                cookie.Value = culture;
            }
            else
            {
                // create cookie value 
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
            }

            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }
    }
}