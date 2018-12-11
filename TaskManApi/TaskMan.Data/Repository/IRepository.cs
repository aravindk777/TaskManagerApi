using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TaskMan.Data.Repository
{
    public interface IRepository<T> where T : class
    {
        T Get(int identifier);
        IEnumerable<T> GetAll();
        IEnumerable<T> Search(Expression<Func<T, bool>> query);
        int Add(T entity);
        int Delete(T entity);
        int Update(T entity);
    }
}
