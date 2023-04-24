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
    public class PhoneRepository : Repository<Phone>, IPhoneRepository
    {
        private readonly ApplicationDbContext _db;
        public PhoneRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Phone obj)
        {
            var objFromDb = _db.phones.FirstOrDefault(u => u.Id == obj.Id);
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
