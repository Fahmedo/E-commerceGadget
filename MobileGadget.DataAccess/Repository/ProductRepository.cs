using MobileGadget.DataAccess.Data;
using MobileGadget.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileGadget.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product obj)
        {

            ////Updating just all of the properties in Product model expect ImageUrl
            var objFromDb = _db.products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.ProductName = obj.ProductName;
                objFromDb.About = obj.About;
                objFromDb.Price = obj.Price;
                objFromDb.BatteryCapacity = obj.BatteryCapacity;
                objFromDb.Storage = obj.Storage;
                objFromDb.Microprocessor = obj.Microprocessor;
                objFromDb.HardDrive = obj.HardDrive;
                objFromDb.ProductModel = obj.ProductModel;
                objFromDb.Condition = obj.Condition;
                objFromDb.Color = obj.Color;
                objFromDb.PhoneId = obj.PhoneId;
                objFromDb.LaptopId = obj.LaptopId;

                // updating  the imageurl property if only it's populated 
                // that is if we have imageurl already and we just want to update a new one
                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }


            }


        }
    }
}
