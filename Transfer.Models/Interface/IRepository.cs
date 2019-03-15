using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Transfer.Models.Interface
{
    public interface IRepository<TEntity> : IDisposable
         where TEntity : class
    {

        TEntity Create(TEntity instance);

        void Update(TEntity instance);

        void Update(TEntity instance, params Expression<Func<TEntity, object>>[] properties);

        void Update(TEntity instance, params object[] keyValues);

        void Delete(TEntity instance);

        TEntity Get(Expression<Func<TEntity, bool>> predicate);


        IQueryable<TEntity> GetSome(Expression<Func<TEntity, bool>> predicate);

        TEntity Find(int idx);

        IQueryable<TEntity> GetAll();

        IEnumerable<TEntity> SqlQuery();

        void SaveChanges();
    }
}
