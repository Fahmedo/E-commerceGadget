using MobileGadget.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileGadget.DataAccess.Repository.IRepository
{
    public interface ITechnicianRepository : IRepository<Technician>
    {
        void Update(Technician obj);
    }
}
