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
    public class LaptopRepository : Repository<Laptop>, ILaptopRepository
    {
        private readonly ApplicationDbContext _db;
        public LaptopRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Laptop obj)
        {
            var objFromDb = _db.laptops.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.OperatingSystem = obj.OperatingSystem;
                objFromDb.DisplayOrder = obj.DisplayOrder;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
