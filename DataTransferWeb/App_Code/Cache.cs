using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Configuration;

namespace DataTransferWeb
{
    public static class Cache
    {
        private static string sessionType
        {
            get
            {
                return ConfigurationManager.AppSettings["sessionType"].ToString();
            }
        }

        private static string redisServer
        {
            get
            {
                return ConfigurationManager.AppSettings["redisServer"].ToString();
            }
        }
        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            //return ConnectionMultiplexer.Connect("CSFRedis.redis.cache.windows.net:6380,password=XYKyVraCLqG24oJjezHYxH4+PiB8R46rd4/csvwB/Z0=,ssl=True,abortConnect=False,allowAdmin=true");
            return ConnectionMultiplexer.Connect(redisServer);
        });

        public static ConnectionMultiplexer redisConn
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        /// <summary>
        /// 快取資料並指定過期時間
        /// </summary>
        public static void SetLimitedCache(string key, dynamic value)
        {
            key = "p" + key.Replace("-", "").Replace("+", "").Replace("=", "");
            string strValue = JsonConvert.SerializeObject(value);
            if (sessionType.Equals("local"))
            {
                System.Web.HttpContext.Current.Session[key] = strValue;
            }
            else if (sessionType.Equals("redis"))
            {
                // 取得 Redis Database
                var redisDatabase = redisConn.GetDatabase();
                // 快取 360分鐘  後過期字串資料
                redisDatabase.StringSet(key, strValue, TimeSpan.FromMinutes(360));
            }
        }

        /// <summary>
        /// 取得快取資料
        /// </summary>
        public static string GetCache(string key)
        {
            key = "p" + key.Replace("-", "").Replace("+", "").Replace("=", "");
            try
            {
                if (sessionType.Equals("local"))
                {
                    return System.Web.HttpContext.Current.Session[key].ToString();
                }
                else if (sessionType.Equals("redis"))
                {
                    // 取得 Redis Database
                    var redisDatabase = redisConn.GetDatabase();
                    // 取得快取資料
                    var value = redisDatabase.StringGet(key);
                    return value;
                }
                else
                    return null;
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
                if (sessionType.Equals("local"))
                {
                    return JsonConvert.DeserializeObject<string>(System.Web.HttpContext.Current.Session[key].ToString());
                }
                else if (sessionType.Equals("redis"))
                {
                    // 取得 Redis Database
                    var redisDatabase = redisConn.GetDatabase();
                    // 取得快取資料
                    var value = redisDatabase.StringGet(key);
                    var timespan = redisDatabase.KeyTimeToLive(key);
                    return JsonConvert.DeserializeObject<string>(value);
                }
                else
                    return null;
            }
            catch { return null; }
        }

        /// <summary>
        /// 刪除快取資料
        /// </summary>
        public static void DelCache(string key)
        {
            key = "p" + key.Replace("-", "").Replace("+", "").Replace("=", "");
            if (sessionType.Equals("local"))
            {
                System.Web.HttpContext.Current.Session.Remove(key);
            }
            else if (sessionType.Equals("redis"))
            {
                // 取得 Redis Database
                var redisDatabase = redisConn.GetDatabase();
                // 取得快取資料
                redisDatabase.KeyDelete(key);
            }
        }

        /// <summary>
        /// 清空快取
        /// </summary>
        public static void DelAllCache()
        {
            if (sessionType.Equals("local"))
            {
                System.Web.HttpContext.Current.Session.RemoveAll();
            }
            else if (sessionType.Equals("redis"))
            {
                // 取得 Redis Database
                var redisDatabase = redisConn.GetDatabase();
                // 刪除快取資料
                try
                {
                    redisDatabase.KeyDelete(ManagerHelper.Guid + "ClassRoomNo");
                    redisDatabase.KeyDelete(ManagerHelper.Guid + "ExamPlaceNo");
                    redisDatabase.KeyDelete(ManagerHelper.Guid + "ExamPlaceName");
                    redisDatabase.KeyDelete(ManagerHelper.Guid + "ManageID");
                    redisDatabase.KeyDelete(ManagerHelper.Guid + "ManageName");
                    redisDatabase.KeyDelete(ManagerHelper.Guid + "Date");
                }catch { }

                /// 清除整個 redis database
                //var endpoints = redisConn.GetEndPoints(true);
                //foreach (var endpoint in endpoints)
                //{
                //    var server = redisConn.GetServer(endpoint);
                //    server.FlushDatabase();
                //}
            }
        }
    }
}