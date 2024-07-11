using Bulky.DataAccess.Migrations;
using Bulky.DataAccess.Repository.IRepository;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<Bulky.Models.Models.ApplicationUser>, IApplicationUserRepository
    {
        public ApplicationDbContext DB { get; set; }   
        public ApplicationUserRepository(ApplicationDbContext _db) : base(_db)
        {
            DB = _db;

        }
        public void Update(ApplicationUser applicationUser)
        {
            _db.Update(applicationUser);
        }
    }
}
