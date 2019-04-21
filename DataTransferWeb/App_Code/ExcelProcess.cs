using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Transfer.Models;
using Transfer.Models.Repository;

namespace DataTransferWeb
{
    public class ExcelProcess
    {
        static List<vwCodeMapping> codeMap = new List<vwCodeMapping>();

        /// <summary>
        /// 輸出Excel(重複不顯示)
        /// </summary>
        /// <param name="FileName">輸出檔名</param>
        /// <param name="sheetName">Sheet名稱</param>
        /// <param name="Datas">填入之資料</param>
        /// <param name="mappings">填入之資料</param>
        public static HSSFWorkbook GenerateExcel(DataTable dt, List<tblExcelMapping> mappings)
        {
            string ExcelName = mappings[0].ExcelName;
            // 取得代碼轉換資料
            using (vwCodeMappingRepository rep = new vwCodeMappingRepository())
            {
                codeMap = rep.query("", "EXPORT", "EXCEL", ExcelName, "");
            }

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
                            
                            // 代碼轉換
                            if (codeMap.Where(x => x.FieldName.Equals(Columns[i].ColumnName, StringComparison.OrdinalIgnoreCase)).Count() > 0
                             && codeMap.Where(x => x.BeforeValue.Equals(value, StringComparison.OrdinalIgnoreCase)).Count() > 0)
                            {
                                vwCodeMapping map = codeMap.Find(x => x.FieldName.Equals(Columns[i].ColumnName, StringComparison.OrdinalIgnoreCase) && x.BeforeValue.Equals(value, StringComparison.OrdinalIgnoreCase));
                                if (map != null)
                                    value = map.AfterValue;
                            }

                            if (!string.IsNullOrEmpty(Columns[i].NewLineChar)) value = value.Replace(Columns[i].NewLineChar, "\n");
                        }

                        // 沒有指定 Field 時，代入 Default Value
                        if (string.IsNullOrEmpty(Columns[i].FieldName))
                        {
                            ICell cell = dataRow.CreateCell(X);
                            cell.CellStyle = dataStyle;
                            SetCellValue(ref cell, Columns[i].DataType, value);
                        }
                        // 判斷是否和上筆資料一致
                        else if (row == 0 || Columns[i].CanRepeat || (dt.Rows[row][Columns[i].FieldName].ToString() != dt.Rows[row - 1][Columns[i].FieldName].ToString()))
                        {
                            ICell cell = dataRow.CreateCell(X);
                            cell.CellStyle = dataStyle;
                            SetCellValue(ref cell, Columns[i].DataType, value);
                        }
                    }
                    sheet.AutoSizeColumn(X);
                }
                #endregion
            }

            return book;
        }


        /// <summary>
        /// 輸出Excel(合併儲存格)
        /// </summary>
        /// <param name="FileName">輸出檔名</param>
        /// <param name="sheetName">Sheet名稱</param>
        /// <param name="Datas">填入之資料</param>
        /// <param name="mappings">填入之資料</param>
        public static HSSFWorkbook GenerateExcelWithCombine(DataTable dt, List<tblExcelMapping> mappings)
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
                        // 判斷是否和上筆資料一致
                        else if (row == 0 || !Columns[i].CanRepeat || (dt.Rows[row][Columns[i].FieldName].ToString() != dt.Rows[row - 1][Columns[i].FieldName].ToString()))
                        {
                            if (fixRow > 0)
                                sheet.AddMergedRegion(new CellRangeAddress(fixRow, row, i, i));

                            ICell cell = dataRow.CreateCell(X);
                            cell.CellStyle = dataStyle;
                            SetCellValue(ref cell, Columns[i].DataType, value);
                            fixRow = -1;
                        }
                        // 是的話合併儲存格
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