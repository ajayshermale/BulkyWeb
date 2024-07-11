using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Bulky.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public ApplicationDbContext _db;
        public DbSet<T> Dbset;


        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.Dbset = _db.Set<T>();
            _db.Products.Include(u => u.Category);
        }

        public void Add(T entity)
        {
            Dbset.Add(entity);
        }

        public T Get(Expression<Func<T, bool>> Filter, string? includeProperties = null, bool tracked = false)
        {
            IQueryable<T> query;
            if (tracked)
            {
                query = Dbset;

            }
            else
            {
                query = Dbset.AsNoTracking();
            }
            query = query.Where(Filter);
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includePror in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includePror);
                }

            }
            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? Filter=null, string? includeProperties = null)
        {
            IQueryable<T> query = Dbset;
            if (Filter != null)
            {
                query = query.Where(Filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeprop in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity)
        {
            Dbset.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            Dbset.RemoveRange(entities);
        }
    }
}
