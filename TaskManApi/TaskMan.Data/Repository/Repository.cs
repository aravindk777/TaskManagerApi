﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
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
                if (Context.Entry(entity).State == EntityState.Detached)
                    Context.Entry(entity).State = EntityState.Added;
                Context.Set<T>().AddOrUpdate(entity);
                return Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int Delete(T entity)
        {
            try
            {
                Context.Set<T>().Remove(entity);
                return Context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
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
                //Context.Set<T>().Attach(entity);
                //Context.Entry(entity).State = EntityState.Modified;

                Context.Set<T>().AddOrUpdate(entity);
                //Context.Entry(entity).CurrentValues.SetValues(entity);
                return Context.SaveChanges();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
