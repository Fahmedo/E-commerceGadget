using MobileGadget.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MobileGadget.Model;

using Microsoft.EntityFrameworkCore;
using MobileGadget.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileGadget.Model;
using MobileGaget.Model;
using MobileGadget.Model;

namespace MobileGadget.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
       public DbSet<Laptop> laptops { get; set; }
        public DbSet<Phone> phones { get; set; }

        public DbSet<Product> products { get; set; }
        public DbSet<Technician> technicians { get; set; }
           public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
      public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
