using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Transfer.Models;

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
            Encoding enc = Encoding.GetEncoding("big5");

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

        //回傳重複的 "字串" 所組成的字串
        public static string strRepeat(string stringToRepeat, int repeat)
        {
            var builder = new StringBuilder(repeat * stringToRepeat.Length);
            for (int i = 0; i < repeat; i++)
            {
                builder.Append(stringToRepeat);
            }
            return builder.ToString();
        }

        public static XmlDocument GenerateXML(DataTable dt, List<tblXMLMapping> xmlMappings)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //根節點 只有1個
            var rootTag = xmlMappings.Where(x => string.IsNullOrEmpty(x.FatherTag)).First();

            XmlElement root = xmlDoc.CreateElement(rootTag.TagName);
            for (int i = 0; i < dt.Rows.Count; i++)
                appendXmlByRow(dt.Rows[i], xmlMappings, xmlDoc, root, rootTag.TagName, 1);

            //addSubElement(xmlMappings, xmlDoc, root, rootTag.TagName, dt);
            xmlDoc.AppendChild(root);

            return xmlDoc;
        }

        static void appendXmlByRow(DataRow row, List<tblXMLMapping> mappings, XmlDocument xmlDoc, XmlElement Node, string FatherTag, int layer)
        {
            // 根節點
            var roots = mappings.Where(x => x.FatherTag.Equals(FatherTag, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.Idx);
            foreach (var root in roots)
            {
                string value = string.Empty;
                if (!string.IsNullOrEmpty(root.FieldName) || !string.IsNullOrEmpty(root.DefaultValue))
                {
                    if (string.IsNullOrEmpty(root.FieldName))
                        value = root.DefaultValue;
                    else
                        value = (string.IsNullOrEmpty(row[root.FieldName].ToString())) ? root.DefaultValue : row[root.FieldName].ToString();
                }

                XmlNode node = Node.SelectSingleNode(strRepeat("/", layer) + root.TagName);
                if (node == null)
                {
                    XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                    if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                    appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);

                    Node.AppendChild(rootNode);
                }
                else
                {
                    XmlNodeList nodes = Node.GetElementsByTagName(root.TagName);

                    //// 單純為階層節點時
                    if (string.IsNullOrEmpty(root.FieldName) && string.IsNullOrEmpty(root.DefaultValue) && root.CanRepeat)
                    {
                        XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                        if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                        appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);
                        Node.AppendChild(rootNode);
                    }
                    else
                    {
                        int nodeCount = 0;
                        if (!string.IsNullOrEmpty(root.FieldName))
                            nodeCount = nodes.Cast<XmlNode>().Where(n => n.InnerText == row[root.FieldName].ToString()).Count();
                        if ((node == null) || (nodes.Count > 0 && !string.IsNullOrEmpty(root.FieldName) && nodeCount == 0))
                        {
                            XmlElement rootNode = xmlDoc.CreateElement(root.TagName);
                            if (!string.IsNullOrEmpty(value)) rootNode.InnerText = value;
                            appendXmlByRow(row, mappings, xmlDoc, rootNode, root.TagName, 1);
                            Node.AppendChild(rootNode);
                        }
                        else
                        {
                            XmlElement element = nodes.Cast<XmlElement>().FirstOrDefault();
                            appendXmlByRow(row, mappings, xmlDoc, element, root.TagName, 2);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 輸出Excel
        /// </summary>
        /// <param name="FileName">輸出檔名</param>
        /// <param name="sheetName">Sheet名稱</param>
        /// <param name="Datas">填入之資料</param>
        /// <param name="mappings">填入之資料</param>
        public static HSSFWorkbook GenerateExcel(DataTable dt, List<tblExcelMapping> mappings)
        {
            HSSFWorkbook book = new HSSFWorkbook();

            IEnumerable<string> sheetNames = mappings.Select(x => x.SheetName).Distinct();
            foreach (var name in sheetNames)
            {
                ISheet sheet;
                sheet = (HSSFSheet)book.GetSheet(name);
                if (sheet == null)
                    sheet = (HSSFSheet)book.CreateSheet(name);
                sheet.DisplayZeros = true;

                List<tblExcelMapping> Columns = mappings.Where(x => x.SheetName.Equals(name, StringComparison.OrdinalIgnoreCase)).OrderBy(x => x.X).ToList();

                #region 表頭
                IRow headerrow = sheet.CreateRow(0);
                ICellStyle style = book.CreateCellStyle();
                style.Alignment = HorizontalAlignment.Center;
                style.VerticalAlignment = VerticalAlignment.Center;
                for (int i = 0; i < Columns.Count(); i++)
                {
                    int X = Columns[i].X;
                    ICell cell = headerrow.CreateCell(X);
                    cell.CellStyle = style;
                    cell.SetCellValue(Columns[i].ColumnName);
                }
                #endregion

                #region 填入內容
                ////顯示有逗號區分的數值資料
                //HSSFCellStyle styleNumeric = (HSSFCellStyle)workbook.CreateCellStyle();
                //HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
                //styleNumeric.DataFormat = format.GetFormat("###,##0");
                //cell.CellStyle = styleNumeric;

                ////顯示百分比符號 
                //HSSFCellStyle stylePercent = (HSSFCellStyle)workbook.CreateCellStyle();
                //HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
                //stylePercent.DataFormat = format.GetFormat("#0.00%");
                //cell.CellStyle = stylePercent;

                ////顯示日期格式
                //HSSFCellStyle styleDate = (HSSFCellStyle)workbook.CreateCellStyle();
                //HSSFDataFormat format = (HSSFDataFormat)workbook.CreateDataFormat();
                //styleDate.DataFormat = format.GetFormat("yyyy-mm-dd");
                //cell.CellStyle = styleDate;


                for (int i = 0; i < Columns.Count(); i++)
                {
                    int X = Columns[i].X;
                    ICellStyle dataStyle = book.CreateCellStyle();
                    dataStyle.Alignment = HorizontalAlignment.Center;
                    dataStyle.VerticalAlignment = VerticalAlignment.Center;
                    if (!string.IsNullOrEmpty(Columns[i].NewLineChar)) dataStyle.WrapText = true;   // 有設定換行字元時，啟動自動換行

                    if (Columns[i].DataType == "Date")
                    {
                        //顯示日期格式
                        HSSFDataFormat dateFormat = (HSSFDataFormat)book.CreateDataFormat();
                        dataStyle.DataFormat = dateFormat.GetFormat("yyyy-MM-dd");
                    }
                    if (Columns[i].DataType == "DateTime")
                    {
                        ////顯示日期格式
                        HSSFDataFormat dateFormat = (HSSFDataFormat)book.CreateDataFormat();
                        dataStyle.DataFormat = dateFormat.GetFormat("yyyy-MM-dd HH:mm:ss");                        
                    }
                    else if (Columns[i].DataType == "Integer")
                    {
                        ////顯示有逗號區分的數值資料
                        HSSFDataFormat numericformat = (HSSFDataFormat)book.CreateDataFormat();
                        dataStyle.DataFormat = numericformat.GetFormat("###,##0");
                    }
                    else if (Columns[i].DataType == "Decimal")
                    {
                        ////顯示有逗號區分的小數資料
                        HSSFDataFormat decimalFormat = (HSSFDataFormat)book.CreateDataFormat();
                        dataStyle.DataFormat = decimalFormat.GetFormat("###,##0.0000");
                    }

                    int fixRow = -1;
                    for (int row = 0; row < dt.Rows.Count; row++)
                    {
                        IRow dataRow;
                        if (i == 0)
                            dataRow = sheet.CreateRow(row + 1);
                        else
                            dataRow = sheet.GetRow(row + 1);

                        string value = string.Empty;
                        if (!string.IsNullOrEmpty(Columns[i].FieldName) || !string.IsNullOrEmpty(Columns[i].DefaultValue))
                        {
                            if (string.IsNullOrEmpty(Columns[i].FieldName))
                                value = Columns[i].DefaultValue;
                            else
                                value = (string.IsNullOrEmpty(dt.Rows[row][Columns[i].FieldName].ToString())) ? Columns[i].DefaultValue : dt.Rows[row][Columns[i].FieldName].ToString();

                            if (!string.IsNullOrEmpty(Columns[i].NewLineChar)) value = value.Replace(Columns[i].NewLineChar, "\n");
                        }

                        // 沒有指定 Field 時，代入 Default Value
                        if (string.IsNullOrEmpty(Columns[i].FieldName))
                        {
                            ICell cell = dataRow.CreateCell(X);
                            cell.CellStyle = dataStyle;
                            SetCellValue(ref cell, Columns[i].DataType, value);
                        }
                        // 判斷是否和上筆資料一致，是的話合併儲存格
                        else if (row == 0 || (dt.Rows[row][Columns[i].FieldName].ToString() != dt.Rows[row - 1][Columns[i].FieldName].ToString()))
                        {
                            if (fixRow > 0)
                                sheet.AddMergedRegion(new CellRangeAddress(fixRow, row, i, i));

                            ICell cell = dataRow.CreateCell(X);
                            cell.CellStyle = dataStyle;
                            SetCellValue(ref cell, Columns[i].DataType, value);
                            fixRow = -1;
                        }
                        else if (fixRow < 0)
                        {
                            fixRow = row;
                        }
                    }
                    if (fixRow > 0)
                        sheet.AddMergedRegion(new CellRangeAddress(fixRow, dt.Rows.Count, i, i));

                    sheet.AutoSizeColumn(X);
                }
                #endregion
            }

            return book;
        }

        static void SetCellValue(ref ICell cell, string DataType, string value)
        {
            try
            {
                switch (DataType)
                {
                    case "Date":
                    case "DateTime":
                        DateTime date = Convert.ToDateTime(value);
                        cell.SetCellValue(date);
                        break;
                    case "Integer":
                    case "Decimal":
                        double doubleValue = Convert.ToDouble(value);
                        cell.SetCellValue(doubleValue);
                        break;
                    case "String":
                        cell.SetCellValue(value);
                        break;
                }
            }
            catch
            {
                cell.SetCellValue(value);
            }
        }
    }
}