using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using TDDlearn2.Store;

namespace TddStore.Core
{
    public class OrderService
    {
        private IOrderDataService _orderDataService;
        private ICustomerService customerService;

        private IOrderFulfillmentService _orderFulfillmentService;
        private const string USERNAME = "Bob";
        private const string PASSWORD = "Foo";


        public OrderService(IOrderDataService orderDataService,ICustomerService _customerService,
            IOrderFulfillmentService orderFulfillmentService)
        {
            _orderDataService = orderDataService;
            customerService = _customerService;
            _orderFulfillmentService = orderFulfillmentService;
        }

        public object PlaceOrder(Guid customerId, ShoppingCart shoppingCart)
        {
            if (shoppingCart.Items.Any(x=>x.Quantity==0))
            {
                return new BadRequestResult();
            }

            var customer = customerService.GetCustomer(customerId);

            PlaceOrderWithFulfillmentService(shoppingCart, customer);

            var order = new Order();
            return _orderDataService.Save(order);
        }

        private void PlaceOrderWithFulfillmentService(ShoppingCart shoppingCart, Customer customer)
        {
            //Open Session
            var orderFulfillmentSessionId = OpenOrderFulfillmentSession();

            PlaceOrderWithFulfillmentService(orderFulfillmentSessionId, shoppingCart, customer);

            //Close Session
            CloseOrderFulfillmentService(orderFulfillmentSessionId);
        }

        private void PlaceOrderWithFulfillmentService(Guid orderFulfillmentSessionId, ShoppingCart shoppingCart, Customer customer)
        {
            var firstItemId = shoppingCart.Items[0].ItemId;
            var firstItemQuantity = shoppingCart.Items[0].Quantity;

            //Check Inventory Level
            var itemIsInInventory = _orderFulfillmentService.IsInInventory(orderFulfillmentSessionId, firstItemId, firstItemQuantity);

            //Place Orders
            var orderForFulfillmentService = new Dictionary<Guid, int>();
            orderForFulfillmentService.Add(firstItemId, firstItemQuantity);
            var orderPlaced = _orderFulfillmentService.PlaceOrder(orderFulfillmentSessionId,
                orderForFulfillmentService,
                customer.ShippingAddress.ToString());
        }

        private void CloseOrderFulfillmentService(Guid orderFulfillmentSessionId)
        {
            _orderFulfillmentService.CloseSession(orderFulfillmentSessionId);
        }

        private Guid OpenOrderFulfillmentSession()
        {
            var orderFulfillmentSessionId = _orderFulfillmentService.OpenSession(USERNAME, PASSWORD);
            return orderFulfillmentSessionId;
        }

    }
}