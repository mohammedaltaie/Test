using Microsoft.EntityFrameworkCore;
using ProductsandCategories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginAndRegistration.Models
{
    public class MyDBContext : DbContext
    {
        public MyDBContext()
        { }
        public MyDBContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Products> Productss { get; set; }
        public DbSet<Categories> Categoriess { get; set; }
        public DbSet<Products_and_Categories> Productss_and_Categoriess { get; set; }


    }
}
