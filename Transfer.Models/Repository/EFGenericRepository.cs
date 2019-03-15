using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Transfer.Models.Repository
{
    /// <summary>
    /// 實作Entity Framework Generic Repository 的 Class。
    /// </summary>
    /// <typeparam name="TEntity">EF Model 裡面的Type</typeparam>
    public class EFGenericRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private DbContext Context { get; set; }

        /// <summary>
        /// 建構EF一個Entity的Repository，需傳入此Entity的Context。
        /// </summary>
        /// <param name="inContext">Entity所在的Context</param>
        public EFGenericRepository(DbContext inContext)
        {
            Context = inContext;
        }

        /// <summary>
        /// 新增一筆資料到資料庫。
        /// </summary>
        /// <param name="entity">要新增到資料的庫的Entity</param>
        public void Create(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        /// <summary>
        /// 取得Entity全部筆數的IQueryable。
        /// </summary>
        /// <returns>Entity全部筆數的IQueryable。</returns>
        public IQueryable<TEntity> Reads()
        {
            return Context.Set<TEntity>().AsQueryable();
        }

        /// <summary>
        /// 取得第一筆符合條件的內容。如果符合條件有多筆，也只取得第一筆。
        /// </summary>
        /// <param name="predicate">要取得的Where條件。</param>
        /// <returns>取得第一筆符合條件的內容。</returns>
        public TEntity Read(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).FirstOrDefault();
            //or
            //return this.Context.Set<TEntity>().FirstOrDefault(predicate);
        }

        /// <summary>
        /// 取得key值為idx的資料
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public TEntity Get(int idx)
        {
            return this.Context.Set<TEntity>().Find(idx);
        }

        /// <summary>
        /// 取得符合條件的內容
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetSome(Expression<Func<TEntity, bool>> predicate)
        {
            return (IQueryable<TEntity>)this.Context.Set<TEntity>().Where(predicate);
        }

        /// <summary>
        /// 從既有資料集中取得符合條件的內容(如果source為null時，則從整個資料表中取得)
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IQueryable<TEntity> GetSome(IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate)
        {
            if (source == null)
                return GetSome(predicate);
            else
                return (IQueryable<TEntity>)source.Where(predicate);
        }


        /// <summary>
        /// 更新一筆Entity內容。
        /// </summary>
        /// <param name="entity">要更新的內容</param>
        public void Update(TEntity entity)
        {
            Context.Entry<TEntity>(entity).State = EntityState.Modified;
        }

        /// <summary>
        /// 更新一筆Entity的內容。只更新有指定的Property。
        /// </summary>
        /// <param name="entity">要更新的內容。</param>
        /// <param name="updateProperties">需要更新的欄位。</param>
        public void Update(TEntity entity, Expression<Func<TEntity, object>>[] updateProperties)
        {
            Context.Configuration.ValidateOnSaveEnabled = false;

            Context.Entry<TEntity>(entity).State = EntityState.Unchanged;

            if (updateProperties != null)
            {
                foreach (var property in updateProperties)
                {
                    Context.Entry<TEntity>(entity).Property(property).IsModified = true;
                }
            }
        }

        /// <summary>
        /// 刪除一筆資料內容。
        /// </summary>
        /// <param name="entity">要被刪除的Entity。</param>
        public void Delete(TEntity entity)
        {
            Context.Entry<TEntity>(entity).State = EntityState.Deleted;
        }

        /// <summary>
        /// 儲存異動。
        /// </summary>
        public void SaveChanges()
        {
            Context.SaveChanges();

            // 因為Update 單一model需要先關掉validation，因此重新打開
            if (Context.Configuration.ValidateOnSaveEnabled == false)
            {
                Context.Configuration.ValidateOnSaveEnabled = true;
            }
        }
    }
}
