using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

public class ManagerHelper
{
    //var userData = new
    //{
    //    account = data.Account,
    //    name = data.Name,
    //    role = data.UserType,
    //    date = data.Date,
    //};

    public static string Guid
    {
        get
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return HttpContext.Current.User.Identity.Name;
            }
            else
                return "";
        }
    }
    public static string Account
    {
        get
        {
            if (string.IsNullOrEmpty(UserData)) return "";
            dynamic obj = JsonConvert.DeserializeObject(UserData);
            if (obj != null)
            {
                return obj.account.ToString();
            }
            return "";
        }
    }

    public static string UserName
    {
        get
        {
            if (string.IsNullOrEmpty(UserData)) return "";
            dynamic obj = JsonConvert.DeserializeObject(UserData);
            if (obj != null)
            {
                return obj.name.ToString();
            }
            return "";
        }
    }
    
    public static DateTime Date
    {
        get
        {
            if (string.IsNullOrEmpty(UserData)) return DateTime.Today.Date;
            
            dynamic obj = JsonConvert.DeserializeObject(UserData);
            if (obj != null)
            {
                return Convert.ToDateTime(obj.date);
            }
            return DateTime.Today.Date.AddDays(-10);
        }
    }

    private static string UserData
    {
        get
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                // 先取得使用者的 FormsIdentity
                FormsIdentity id = HttpContext.Current.User.Identity as FormsIdentity;
                // 取出使用者的 FormsAuthenticationTicket
                FormsAuthenticationTicket ticket = id.Ticket;
                var userInfo = id.Ticket.UserData;

                return userInfo;
            }
            else
                return "";
        }
    }
    public static string GetUserData()
    {
        if (HttpContext.Current.User.Identity.IsAuthenticated)
        {
            // 先取得使用者的 FormsIdentity
            FormsIdentity id = HttpContext.Current.User.Identity as FormsIdentity;
            // 取出使用者的 FormsAuthenticationTicket
            FormsAuthenticationTicket ticket = id.Ticket;
            var userInfo = id.Ticket.UserData;

            return userInfo;
        }
        return "";
    }
}
