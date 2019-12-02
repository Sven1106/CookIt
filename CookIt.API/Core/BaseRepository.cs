using CookIt.API.Data;
using CookIt.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CookIt.API.Core
{
    public class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DbContext Context { get; private set; }

        protected DbSet<TEntity> Set => Context.Set<TEntity>();

        public BaseRepository(DbContext dbContext)
        {
            Context = dbContext;
        }

        public IQueryable<TEntity> Query // Not sure if the Set.Local check is correct.
        {
            get
            {
                return Set.Local.Count != 0 ?
                    Set.Local.AsQueryable() :
                    Set.AsQueryable();
            }
        }

        public TEntity Find(params object[] keys)
        {
            return Set.Find(keys);
        }

        public void Insert(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Set.Add(entity);
        }

        public void Delete(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Set.Attach(entity);
            Set.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Set.Attach(entity);
            entry.State = EntityState.Modified;
        }

    }
}
