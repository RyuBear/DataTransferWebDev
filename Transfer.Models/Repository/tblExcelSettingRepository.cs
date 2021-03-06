﻿using System;
using System.Linq;
using Transfer.Models.ViewModel;
using System.Collections.Generic;
using Transfer.Models.Models;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;
using LinqKit;

namespace Transfer.Models.Repository
{
    public class tblExcelSettingRepository : GenericRepository<tblExcelSetting>
    {
        /// <summary>
        /// 確認是否已存在
        /// </summary>
        /// <param name="ExcelName"></param>
        /// <returns></returns>
        public bool isExist(string ExcelName)
        {
            tblExcelSetting setting = this.Get(x => x.ExcelName.Equals(ExcelName, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
                return false;
            else
                return true;
        }

        public tblExcelSetting get(string ExcelName)
        {
            tblExcelSetting setting = this.Get(x => x.ExcelName.Equals(ExcelName, StringComparison.OrdinalIgnoreCase));
            return setting;
        }

        public List<tblExcelSetting> get(string ExcelName, string SQLName, string CustomerName)
        {
            IQueryable<tblExcelSetting> settings = this.GetAll();
            if (!string.IsNullOrEmpty(ExcelName)) settings = this.GetSome(settings, x => x.ExcelName.Contains(ExcelName));
            if (!string.IsNullOrEmpty(SQLName)) settings = this.GetSome(settings, x => x.SQLName.Contains(SQLName));
            if (!string.IsNullOrEmpty(CustomerName)) settings = this.GetSome(settings, x => x.CustomerName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase));

            return settings.ToList();
        }


        public List<tblExcelSetting> getByCustomer(string UserID, string CustomerName)
        {
            IQueryable<tblExcelSetting> settings = this.GetSome(x => x.UserId.Contains(UserID) && x.CustomerName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase));

            return settings.ToList();
        }

        public bool Delete(string ExcelName)
        {
            try
            {
                // 刪除舊有欄位資料
                string _sql = @"Delete From tblExcelMapping Where ExcelName=@ExcelName; 
                                DELETE FROM tblExcelSetting Where ExcelName=@ExcelName";
                SqlParameter[] ps = new SqlParameter[]
                {
                    new SqlParameter("@ExcelName", ExcelName.ToSqlType())
                };
                return this.ExecuteNonQuery(_sql, ps);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 儲存 Excel Setting
        /// </summary>
        /// <returns></returns>
        public string Save(string ExcelName, string CustomerName, string SQLName, string FileName, string DateFormat, string UserID, string Creator, List<tblExcelMapping> Mappings)
        {
            tblExcelSetting setting = this.Get(x => x.ExcelName.Equals(ExcelName, StringComparison.OrdinalIgnoreCase));
            if (setting == null)
            {
                setting = new tblExcelSetting()
                {
                    ExcelName = ExcelName,
                    CustomerName = CustomerName,
                    SQLName = SQLName,
                    FileName = FileName,
                    FileNameDateFormat = DateFormat,
                    UserId = "," + UserID + ",",
                    CreateTime = Now,
                    Creator = Creator
                };
                try
                {
                    this.Create(setting);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
            else
            {
                setting.SQLName = SQLName;
                setting.CustomerName = CustomerName;
                setting.FileName = FileName;
                setting.FileNameDateFormat = DateFormat;
                setting.UserId = "," + UserID + ",";
                setting.UpdateTime = Now;
                setting.Updator = Creator;

                try
                {
                    this.Update(setting);
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }

            // Save Mapping
            using (tblExcelMappingRepository map = new tblExcelMappingRepository())
            {
                return map.Save(ExcelName, Creator, Mappings);
            }
        }
    }
}
