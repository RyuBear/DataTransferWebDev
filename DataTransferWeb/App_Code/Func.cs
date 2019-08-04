using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace DataTransferWeb
{
    public static class Func
    {
        /// <summary>
        /// 轉換DateTime?為字串 (yyyyMMdd)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDate(DateTime? dt)
        {
            return (dt == null) ? "" : dt.Value.ToString("yyyyMMdd");
        }

        /// <summary>
        /// 轉換DateTime?為字串 (yyyyMMddHHmm)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertDateTime(DateTime? dt)
        {
            return (dt == null) ? "" : dt.Value.ToString("yyyyMMddHHmm");
        }

        /// <summary>
        /// 轉換DateTime?為民國字串 (民國xxx年xx月xx日)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ConvertTWDate(DateTime? dt)
        {
            return (dt == null) ? "" : "民國" + (dt.Value.Year - 1911).ToString() + "年" + dt.Value.ToString("MM月dd日");
        }

        public static DateTime GetDateTime()
        {
            DateTime localtime = DateTime.Now;
            TimeZoneInfo TW_TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time");
            DateTime TW_DateTime = TimeZoneInfo.ConvertTime(localtime, TW_TimeZoneInfo);
            return TW_DateTime;
        }

        /// <summary>
        /// 將 HHmm 轉換為 HH:mm
        /// </summary>
        /// <param name="hhmm"></param>
        /// <returns></returns>
        public static string ConvertTime(string hhmm)
        {
            if (CheckDateNum(hhmm, 4))
            {
                return hhmm.Substring(0, 2) + ":" + hhmm.Substring(2, 2);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 將 HH:mm 轉換為 HHmm
        /// </summary>
        /// <returns></returns>
        public static string ConvertToTimeString(string hhmm)
        {
            string[] str = hhmm.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var s in str)
            {
                if (!IsNumeric(s)) return "";
            }
            return str[0].PadLeft(2, '0') + str[1].PadLeft(2, '0');
        }

        /// <summary>
        /// 檢查是否符合台灣身分證字號規則
        /// </summary>
        /// <param name="id" />身分證字號</param>
        /// <returns>True/False</returns>
        public static Boolean CheckIDNO(string id)
        {
            if (string.IsNullOrEmpty(id))
                return false;   //沒有輸入，回傳 ID 錯誤
            id = id.ToUpper();
            var regex = new Regex("^[A-Z]{1}[0-9]{9}$");
            if (!regex.IsMatch(id))
                return false;   //Regular Expression 驗證失敗，回傳 ID 錯誤

            int[] seed = new int[10];       //除了檢查碼外每個數字的存放空間
            string[] charMapping = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "J", "K", "L", "M", "N", "P", "Q", "R", "S", "T", "U", "V", "X", "Y", "W", "Z", "I", "O" };
            //A=10 B=11 C=12 D=13 E=14 F=15 G=16 H=17 J=18 K=19 L=20 M=21 N=22
            //P=23 Q=24 R=25 S=26 T=27 U=28 V=29 X=30 Y=31 W=32  Z=33 I=34 O=35
            string target = id.Substring(0, 1);
            for (int index = 0; index < charMapping.Length; index++)
            {
                if (charMapping[index] == target)
                {
                    index += 10;
                    seed[0] = index / 10;       //10進制的高位元放入存放空間
                    seed[1] = (index % 10) * 9; //10進制的低位元*9後放入存放空間
                    break;
                }
            }
            for (int index = 2; index < 10; index++)
            {   //將剩餘數字乘上權數後放入存放空間
                seed[index] = Convert.ToInt32(id.Substring(index - 1, 1)) * (10 - index);
            }
            //檢查是否符合檢查規則，10減存放空間所有數字和除以10的餘數的個位數字是否等於檢查碼
            //(10 - ((seed[0] + .... + seed[9]) % 10)) % 10 == 身分證字號的最後一碼
            return (10 - (seed.Sum() % 10)) % 10 == Convert.ToInt32(id.Substring(9, 1));
        }

        /// <summary>
        /// 檢查字串是否為數字
        /// </summary>
        /// <param name="vNum" />字串</param>
        /// <returns>True/False</returns>
        public static Boolean IsNumeric(string vNum)
        {
            Boolean rtn = false;

            if (vNum != null)
            {
                for (int ii = 0; ii < vNum.Length; ii++)
                {
                    char c = Convert.ToChar(vNum.Substring(ii, 1));
                    if (!char.IsNumber(c))
                    {
                        rtn = false;
                        break;
                    }
                    else
                    {
                        rtn = true;
                    }
                }
            }
            return rtn;
        }

        /// <summary>
        /// 檢查字串是否為日期
        /// </summary>
        /// <param name="vDate" />字串</param>
        /// <param name="vType" />日期Template -- 1."YYYMMDD"</param>
        /// <returns>True/False</returns>
        public static Boolean IsDate(string vDate, string vType)
        {
            Boolean rtn = true;
            if (vType == "YYYMMDD")
            {
                string YYY = vDate.Substring(0, 3);
                string YYYY = (Convert.ToInt16(YYY) + 1911).ToString();
                string MM = vDate.Substring(3, 2);
                string DD = vDate.Substring(5, 2);

                try
                {
                    DateTime dt = Convert.ToDateTime(YYYY + "/" + MM + "/" + DD);
                }
                catch (Exception ex)
                {
                    rtn = false;
                }
            }

            return rtn;
        }

        /// <summary>
        /// 檢查日期欄位是否為數字
        /// </summary>
        /// <param name="vDate" />日期欄位</param>
        /// <param name="vLength" />日期長度</param>
        /// <returns>True/False</returns>
        public static Boolean CheckDateNum(string vDate, int vLength)
        {
            Boolean rtn = true;
            if (vDate.Length != vLength)
                rtn = false;
            else if (!IsNumeric(vDate))
                rtn = false;

            return rtn;
        }

        /// <summary>
        /// 檢查SQL Statement是否合法
        /// </summary>
        /// <param name="strSQL" />SQL Statement是否合法</param>
        /// <returns>True/False</returns>
        public static Boolean SQLIsValid(string strSQL)
        {
            string sql = (string.IsNullOrEmpty(strSQL)) ? "" : strSQL.ToUpper();
            // 不可含 Update、Delete
            if (strSQL != null)
            {
                if (sql.Contains("UPDATE ") || sql.Contains("DELETE ") || sql.Contains("ALTER ") || sql.Contains("TRUNCATE TABLE") || sql.Contains("CREATE "))
                    return false;
                else if (!sql.Contains("SELECT "))
                    return false;
                else
                    return true;
            }
            else
                return false;
        }


        /// <summary>
        /// 將 SQL 插入 top (N)
        /// </summary>
        /// <param name="strSQL" />SQL Statement是否合法</param>
        /// <returns>True/False</returns>
        public static string SqlPlusTop(string strSQL, int Row)
        {
            // 將 top(n) 帶入 SQL語句
            //int index = strSQL.IndexOf("Select ", StringComparison.OrdinalIgnoreCase);  // 找出第一個select的位置

            int index = strSQL.IndexOf(" Order by ", StringComparison.OrdinalIgnoreCase);  // 找出最後 Order by 的位置
            string sql = "SELECT TOP (" + Row + ") * FROM (" + strSQL + ") T ";
            if (index > 0)
                sql = "SELECT TOP (" + Row + ") * FROM (" + strSQL.Substring(0, index) + ") T " + strSQL.Substring(index);
            return sql.Replace("\r\n", " ");
        }

        /// <summary>
        /// 依XML文件格式顯示
        /// </summary>
        /// <param name="xmldoc"></param>
        /// <returns></returns>
        public static string BeautifyXML(XmlDocument doc)
        {
            Encoding enc = Encoding.GetEncoding("utf-8");

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = false;
            settings.NewLineOnAttributes = true;
            settings.Encoding = enc;

            MemoryStream ms = new MemoryStream();

            XmlWriter writer = XmlWriter.Create(ms, settings);
            doc.WriteTo(writer);
            writer.Flush();

            string result = enc.GetString(ms.ToArray());

            return result;
        }

        //回傳重複的 "字串" 所組成的字串
        public static string Repeat(this string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }

        public static Boolean IsNumeric(string strNum, string split, int lower, int upper)
        {
            Boolean rtn = true;
            if (strNum != null)
            {
                string[] nums = strNum.Split(new string[] { split }, StringSplitOptions.RemoveEmptyEntries);
                for (int ii = 0; ii < nums.Length; ii++)
                {
                    int num = 0;
                    bool isNum = int.TryParse(nums[ii], out num);
                    if (!isNum)
                    {
                        rtn = false;
                        break;
                    }
                    else
                    {
                        if (num < lower || num > upper)
                        {
                            rtn = false;
                            break;
                        }
                    }
                }
            }
            return rtn;
        }

        public static void DelAttachment(FileInfo file)
        {
            try
            {
                if (File.Exists(file.FullName))
                {
                    file.Delete();
                }
            }
            catch { };
        }
    }
}