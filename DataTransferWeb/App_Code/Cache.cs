using Newtonsoft.Json;

namespace DataTransferWeb
{
    public static class Cache
    {
        /// <summary>
        /// 快取資料並指定過期時間
        /// </summary>
        public static void SetLimitedCache(string key, dynamic value)
        {
            key = "p" + key.Replace("-", "").Replace("+", "").Replace("=", "");
            string strValue = JsonConvert.SerializeObject(value);
            System.Web.HttpContext.Current.Session[key] = strValue;
        }

        /// <summary>
        /// 取得快取資料
        /// </summary>
        public static string GetCache(string key)
        {
            key = "p" + key.Replace("-", "").Replace("+", "").Replace("=", "");
            try
            {
                return System.Web.HttpContext.Current.Session[key].ToString();
            }
            catch { return null; }
        }

        /// <summary>
        /// 取得快取資料(反序列化為字串)
        /// </summary>
        public static string GetCacheToString(string key)
        {
            key = "p" + key.Replace("-", "").Replace("+", "").Replace("=", "");
            try
            {
                return JsonConvert.DeserializeObject<string>(System.Web.HttpContext.Current.Session[key].ToString());
            }
            catch { return null; }
        }

        /// <summary>
        /// 刪除快取資料
        /// </summary>
        public static void DelCache(string key)
        {
            key = "p" + key.Replace("-", "").Replace("+", "").Replace("=", "");
            System.Web.HttpContext.Current.Session.Remove(key);
        }

        /// <summary>
        /// 清空快取
        /// </summary>
        public static void DelAllCache()
        {
            System.Web.HttpContext.Current.Session.RemoveAll();
        }
    }
}