using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace Transfer.Models.Repository
{
    public class GenericRepository<TEntity> : DataAccess
        where TEntity : class
    {
        public GenericRepository() : this(new TransferDBEntities()) { }

        public GenericRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.DbContext = context;
        }

        public GenericRepository(ObjectContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            this.DbContext = new DbContext(context, true);
        }

        private DbContext DbContext
        {
            get;
            set;
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
        private void SaveChanges()
        {
            this.DbContext.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.DbContext != null)
                {
                    this.DbContext.Dispose();
                    this.DbContext = null;
                }
            }

            base.Dispose(disposing);
        }

        // 新增，刪除，修改，查詢
        // 執行sql | sp
        // query sql
        // automapper
        // resource management

        // http://msdn.microsoft.com/en-us/data/jj592907.aspx

        public IEnumerable<TEntity> SqlQuery(string sql, params object[] parameters)
        {
            return this.DbContext.Database.SqlQuery<TEntity>(sql, parameters);
        }

        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return this.DbContext.Database.ExecuteSqlCommand(sql, parameters);
        }

        //--------------------------------------------------------------------------------
        // Create, Read, Update, Delete
        //--------------------------------------------------------------------------------
        public TEntity Create(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                TEntity result = this.DbContext.Set<TEntity>().Add(instance);

                this.SaveChanges();
                return result;
            }

        }

        //--------------------------------------------------------------------------------

        public TEntity Get(int idx)
        {
            return this.DbContext.Set<TEntity>().Find(idx);
        }

        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return this.DbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        public IQueryable<TEntity> GetSome(Expression<Func<TEntity, bool>> predicate)
        {
            return (IQueryable<TEntity>)this.DbContext.Set<TEntity>().Where(predicate);
        }

        public IQueryable<TEntity> GetSome(IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate)
        {
            if (source == null)
                return GetSome(predicate);
            else
                return (IQueryable<TEntity>)source.Where(predicate);
        }

        public IQueryable<TEntity> GetAll()
        {
            return (IQueryable<TEntity>)this.DbContext.Set<TEntity>().AsQueryable();
        }

        //--------------------------------------------------------------------------------

        public void Update(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this.DbContext.Entry(instance).State = EntityState.Modified;
                this.SaveChanges();
            }
        }

        public void Update(TEntity instance, params Expression<Func<TEntity, object>>[] properties)
        {
            this.DbContext.Set<TEntity>().Attach(instance);
            DbEntityEntry<TEntity> tempInstance = this.DbContext.Entry(instance);
            foreach (var property in properties)
            {
                tempInstance.Property(property).IsModified = true;
            }
            this.SaveChanges();
        }

        public void Update(TEntity original, TEntity modified)
        {
            if (original == null)
            {
                throw new ArgumentNullException("original");
            }
            else if (modified == null)
            {
                throw new ArgumentNullException("modified");
            }
            else
            {
                this.DbContext.Entry(original).CurrentValues.SetValues(modified);
                this.SaveChanges();
            }
        }

        //--------------------------------------------------------------------------------

        public void Delete(TEntity instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            else
            {
                this.DbContext.Entry(instance).State = EntityState.Deleted;
                this.SaveChanges();
            }
        }

        //--------------------------------------------------------------------------------
        // SerialNumber
        //--------------------------------------------------------------------------------

        public string GetSerialNumber(string TableName, int SeqLen, string BegStr, string EndStr)
        {
            string procedureName = "GetSerialNumber";

            Dictionary<string, object> in_parameters = new Dictionary<string, object>();
            in_parameters.Add("TableName", TableName);
            in_parameters.Add("SeqLen", SeqLen);
            in_parameters.Add("BegStr", BegStr);
            in_parameters.Add("EndStr", EndStr);

            return SP_ExecuteScalar(procedureName, in_parameters) as string;
        }

        public string GetSerialNumber(int SeqLen, string BegStr, string EndStr)
        {
            return GetSerialNumber(typeof(TEntity).Name, SeqLen, BegStr, EndStr);
        }

        public virtual string GetSerialNumber()
        {
            int SeqLen = 10;
            return GetSerialNumber(SeqLen, string.Empty, string.Empty);
        }

        public int SelectIDENT_CURRENT()
        {
            return SelectIDENT_CURRENT(typeof(TEntity).Name);
        }

        /// <summary>
        /// 將 DateTime 轉成字串 (yyyyMMdd)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string ConvertDateTime(DateTime? dt)
        {
            return (dt == null) ? "" : dt.Value.ToString("yyyyMMdd");
        }
        
    }
}
