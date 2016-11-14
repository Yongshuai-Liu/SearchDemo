using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SearchDemo.Repositories.BaseRepositories
{
    public interface IRepository<T> : IDisposable where T : class
    {
        IEnumerable<T> GetAll();
        T GetSingle(int ID);
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Delete(T entity);
        void Update(T entity);
        void Clone(T oldEntity, T newEntity);
        Task<T> FindByAsync(Expression<Func<T, bool>> predicate);

        int SaveChanges();
    }
}
