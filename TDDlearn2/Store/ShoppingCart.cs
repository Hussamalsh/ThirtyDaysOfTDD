﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TddStore.Core
{
    public class ShoppingCart
    {
        public IList<ShoppingCartItem> Items { get; private set; }

        public ShoppingCart()
        {
            Items = new List<ShoppingCartItem>();
        }
    }
}
