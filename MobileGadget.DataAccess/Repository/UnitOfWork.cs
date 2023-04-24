using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using MobileGadget.DataAccess.Data;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MobileGadget.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;

            Product = new ProductRepository(_db);

            Laptop = new LaptopRepository(_db);

            Phone = new PhoneRepository(_db);

            Technician = new TechnicianRepository(_db);

            ApplicationUser = new ApplicationUserRepository(_db);

            ShoppingCart = new ShoppingCartRepository(_db);


            OrderDetail = new OrderDetailRepository(_db);

            OrderHeader = new OrderHeaderRepository(_db);

        }

        public IPhoneRepository Phone { get; private set; }
        public ILaptopRepository Laptop { get; private set; }
        public IProductRepository Product { get; private set; }
        public ITechnicianRepository Technician { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }

        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}