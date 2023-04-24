using BulkyBook.DataAccess.Repository.IRepository;
using MobileGadget.Model;
using MobileGaget.Models;
using MobileGadget.DataAccess.Data;
using MobileGadget.DataAccess.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MobileGadget.Model;

namespace MobileGadget.DataAccess.Repository
{
    public class OrderDetailRepository: Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        //Updating just all of the properties in Category model
        public void Update(OrderDetail obj)
        {

            _db.OrderDetails.Update(obj);
        }
    }
}
