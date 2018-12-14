using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TaskMan.Data.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public int Add(T entity)
        {
            try
            {
                Context.Set<T>().Add(entity);
                return Context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Delete(T entity)
        {
            try
            {
                Context.Set<T>().Remove(entity);
                return Context.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }
        }

        public T Get(int identifier)
        {
            return Context.Set<T>().Find(identifier);
        }

        public IEnumerable<T> GetAll()
        {
            return Context.Set<T>().AsEnumerable();
        }

        public IEnumerable<T> Search(Expression<Func<T, bool>> query)
        {
            return Context.Set<T>().Where(query);
        }

        public int Update(T entity)
        {
            try
            {
                Context.Set<T>().Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
                return Context.SaveChanges();
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
