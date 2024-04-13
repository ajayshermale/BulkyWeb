using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
          _db = db;
        }

        public void update(Product product)
        {
            _db.Update<Product>(product);
        }
    }
}
