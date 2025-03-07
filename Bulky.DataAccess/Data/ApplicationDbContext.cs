﻿
using Bulky.Models;
using Bulky.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

//using BulkyWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.Data
{
    public class ApplicationDbContext :IdentityDbContext<IdentityUser>
    {
        //to pass all the option to base class i.e Dbcontext
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> option ): base(option)
        {
                
        }
        //Tocreate table we need to Use Dbset
        public  DbSet<Category> Categories { get; set; }
        public DbSet<Product>  Products { get; set; }

        public DbSet<Company> Companys { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }  
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Category>().HasData(
        //        new Category { Id = 1 , Name = "Scific", DispalyOrder=1},
        //        new Category { Id = 2, Name = "Hist", DispalyOrder = 2 },
        //        new Category { Id = 3, Name = "Action", DispalyOrder = 3 }
        //        );
        //}

        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
