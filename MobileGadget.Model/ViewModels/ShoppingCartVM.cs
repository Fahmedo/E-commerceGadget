﻿using MobileGadget.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileGadget.Model.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public IEnumerable<ShoppingCart> BuyList { get; set; }

        //putting the orderHeader insdie the shoppingCartVm so we can use the shoopingCartVM
        public OrderHeader OrderHeader { get; set; }
    }
}
