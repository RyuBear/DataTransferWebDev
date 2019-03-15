using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using StackExchange.Redis;
using Newtonsoft.Json;
using System.Configuration;
using DataTransferWeb.Models;

namespace DataTransferWeb
{
    [Authorize] //代表該整個Controller必須要先進行過表單驗證登入後才可以進入
    public class BaseController : Controller
    {
        private static string sessionType
        {
            get
            {
                return ConfigurationManager.AppSettings["sessionType"].ToString();
            }
        }
        public UserData userInfo;
        public List<SelectListItem> ClassRoomItems;


        //public string curExam;

        public BaseController()
        {
            userInfo = new UserData()
            {
                Account = ManagerHelper.Account,
                Name = ManagerHelper.UserName,
                Date = ManagerHelper.Date
            };            
        }

        public DateTime Now
        {
            get
            {
                DateTime localtime = DateTime.Now;
                TimeZoneInfo TW_TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
                DateTime TW_DateTime = TimeZoneInfo.ConvertTime(localtime, TW_TimeZoneInfo);
                return TW_DateTime;
            }
        }    

        public string SystemMsg
        {
            get { return TempData["SystemMsg"] as string; }
            set { TempData["SystemMsg"] = value; }
        }

        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

    }
}