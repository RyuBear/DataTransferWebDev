using System;
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
    public class tblScheduleRepository : GenericRepository<tblSchedule>
    {
        /// <summary>
        /// 確認是否已存在
        /// </summary>
        /// <param name="ScheduleName"></param>
        /// <returns></returns>
        public bool isExist(string ScheduleName)
        {
            tblSchedule s = this.Get(x => x.ScheduleName.Equals(ScheduleName, StringComparison.OrdinalIgnoreCase));
            if (s == null)
                return false;
            else
                return true;
        }


        public tblSchedule get(string ScheduleName)
        {
            tblSchedule s = this.Get(x => x.ScheduleName.Equals(ScheduleName, StringComparison.OrdinalIgnoreCase));
            return s;
        }


        public IEnumerable<tblSchedule> get(string ScheduleName, string CustomerName, string Format)
        {
            IQueryable<tblSchedule> s = this.GetAll();
            if (!string.IsNullOrEmpty(ScheduleName)) s = this.GetSome(s, x => x.ScheduleName.Equals(ScheduleName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(CustomerName)) s = this.GetSome(s, x => x.CustomerName.Equals(CustomerName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(Format)) s = this.GetSome(s, x => x.Format.Equals(Format, StringComparison.OrdinalIgnoreCase));

            return s.ToList();
        }

        public bool Delete(string ScheduleName)
        {
            try
            {
                tblSchedule s = this.Get(x => x.ScheduleName.Equals(ScheduleName, StringComparison.OrdinalIgnoreCase));
                if (s != null)
                    this.Delete(s);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 儲存
        /// </summary>
        /// <returns></returns>
        public string Save(string originName, tblSchedule newS, string Creator)
        {
            try
            {
                tblSchedule s = this.Get(x => x.ScheduleName.Equals(originName, StringComparison.OrdinalIgnoreCase));
                if (s == null)
                {
                    newS.Creator = Creator;
                    newS.CreateTime = Now;
                    this.Create(newS);
                }
                else
                {
                    newS.CreateTime = s.CreateTime;
                    newS.Creator = s.Creator;
                    newS.UpdateTime = Now;
                    newS.Updator = Creator;
                    this.Delete(s);
                    this.Create(newS);
                }
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
