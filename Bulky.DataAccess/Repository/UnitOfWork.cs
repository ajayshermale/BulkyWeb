using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository category {  get; set; }
        public IProductRepository product { get; set; } 

        public ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext Db)
        {
            _db = Db;
            category = new CategoryRepository(_db);
            product=  new ProductRepository(_db);
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
