using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SearchDemo.Repositories.BaseRepositories
{
    public abstract class BaseRepository<T, U> : IRepository<T> where T : class
                                                                where U : DbContext
    {
        protected U _dbContext;

        protected readonly DbSet<T> DbSet;

        protected BaseRepository(U context)
        {
            _dbContext = context;
            DbSet = context.Set<T>();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbContext.Set<T>().AsEnumerable();
        }

        public T GetSingle(int ID)
        {
            return _dbContext.Set<T>().Find(ID);
        }

        public async Task<T> FindByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void Update(T entity)
        {
            try
            {
                var entry = _dbContext.Entry(entity);
                _dbContext.Set<T>().Attach(entity);
                entry.State = EntityState.Modified;
                //SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                // To add information add a variable name to the exception and throw it, Otherwise just use throw to perserve the stack trace
                throw;
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_dbContext != null)
                {
                    _dbContext.Dispose();
                }
            }
        }

        public int SaveChanges()
        {
            try
            {
                return _dbContext.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }

        }


        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _dbContext.Set<T>().Where(predicate).AsEnumerable();
        }

        public void Clone(T oldEntity, T newEntity)
        {
            var sourceValue = _dbContext.Entry(oldEntity).CurrentValues;
            _dbContext.Entry(newEntity).CurrentValues.SetValues(oldEntity);
        }
    }
}
