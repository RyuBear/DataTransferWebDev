using Transfer.Models;
using Transfer.Models.Repository;
using DataTransferWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DataTransferWeb.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        [HttpGet]
        public ActionResult UptPwd()
        {
            UserVM vm = new UserVM();
            return View(vm);
        }

        [HttpPost]
        [ActionName("AjaxUpdate")]
        [ValidateAntiForgeryToken]
        public ActionResult UptPwd(UserVM vm)
        {
            // 限定同網站的Ajax專用
            if (!Request.IsAjaxRequest())
            {
                ModelState.AddModelError("", "非 ajax 呼叫");
            }
            if (!ModelState.IsValid)
            {
                return View("Index");
            }

            using (tblAdminRepository ad = new tblAdminRepository())
            {
                tblAdmin admin = ad.CheckUser(vm.Account, vm.Password);
                if (admin == null)
                {
                    return Json(new { status = "密碼錯誤" });
                }
                else if (!vm.NewPassword.Equals(vm.NewPassword_Confirm))
                {
                    return Json(new { status = "密碼不一致" });
                }
                else
                {
                    bool status = ad.UptPassword(vm.Account, vm.NewPassword);
                    if (status == true)
                    {
                        return Json(new { status = "success" });
                    }
                    else
                    {
                        return Json(new { status = "密碼更新失敗" });
                    }
                }
            }
        }
    }
}