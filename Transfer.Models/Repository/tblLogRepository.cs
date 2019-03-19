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
    public class tblLogRepository : GenericRepository<tblLog>
    {
        public List<tblLog> get(string StartDate, string EndDate, string Status)
        {
            IQueryable<tblLog> logs = this.GetAll();
            DateTime sDate = Convert.ToDateTime(StartDate);
            DateTime eDate = Convert.ToDateTime(EndDate).AddDays(1);
            if (!string.IsNullOrEmpty(StartDate)) logs = this.GetSome(logs, x => x.ExecuteTime.CompareTo(sDate) >= 0);
            if (!string.IsNullOrEmpty(EndDate)) logs = this.GetSome(logs, x => x.ExecuteTime.CompareTo(eDate) <= 0);
            if (!string.IsNullOrEmpty(Status)) logs = this.GetSome(logs, x => x.Status.Equals(Status, StringComparison.OrdinalIgnoreCase));

            return logs.ToList();
        }

        /// <summary>
        /// 寫入 Log
        /// </summary>
        /// <returns></returns>
        public void Save(string Type, string UserID, string CustomerName, string Format, string Destination, string Path, string FileName, string Status, string Message)
        {
            tblLog log = new tblLog()
            {
                Type = Type,
                UserID = UserID,
                CustomerName = CustomerName,
                Format = Format,
                ExecuteTime = Now,
                Destination = Destination,
                Path = Path,
                FileName = FileName,
                Status = Status,
                Message = Message
            };
            try
            {
                this.Create(log);
            }
            catch (Exception ex)
            { }
        }
    }
}
