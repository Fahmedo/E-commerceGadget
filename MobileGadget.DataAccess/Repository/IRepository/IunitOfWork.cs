using BulkyBook.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileGadget.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {

        IProductRepository Product { get; }
        IPhoneRepository Phone { get; }

        ILaptopRepository Laptop { get; }
        ITechnicianRepository Technician { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailRepository OrderDetail { get; }
        void Save();
    }
}
