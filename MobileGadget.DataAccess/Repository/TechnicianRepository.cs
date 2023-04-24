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
    public class TechnicianRepository : Repository<Technician>, ITechnicianRepository
    {
        private readonly ApplicationDbContext _db;
        public TechnicianRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(Technician obj)
        {
            var objFromDb = _db.technicians.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.CompanyName = obj.CompanyName;
                objFromDb.StreetAddress = obj.StreetAddress;
                objFromDb.City = obj.City;
                objFromDb.State = obj.State;
                objFromDb.PostalCode = obj.PostalCode;
                objFromDb.PhoneNumber = obj.PhoneNumber;

                if (obj.ImageUrl != null)
                {
                    objFromDb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
