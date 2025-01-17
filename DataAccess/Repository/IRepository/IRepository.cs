using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        public void Create(T entity);
        public void Alter(T entity);
        public void Delete(T entity);
        public void Commit();
        public IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null, bool tracked = true);
        public T? GetOne(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includeProps = null, bool tracked = true);
    }
}
