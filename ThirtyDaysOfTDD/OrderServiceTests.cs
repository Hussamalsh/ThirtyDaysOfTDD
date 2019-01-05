using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using TDDlearn2.Store;
using TddStore.Core;
/*
Assuming a user has logged into the application and placed items in a shopping cart, the application should enable to user to place an order based on the items in the shopping cart. 
Users should be able to order any number of different items they wish. Users may not order a zero or lower quantity of any item.
If the user attempts to order a quantity of zero or less for any item an exception should be thrown and the entire order aborted (the shopping cart should NOT be emptied). 
Once the order quantity validation rule has been validated the order should be saved to the database via the Orders Database service and linked to the customer who placed the order. 
Calls will be made to the billing and order fulfillment systems to launch their respective workflows. A log entry shall be made to show that the order was placed.
If the order cannot be placed for any reason other than a failure of the validation rules, the error will be logged and an exception will be thrown. 
Upon a successful placement of an order the shopping cart will be emptied and the order id (from the Order Database service) will be returned. 
*/
// items  *   shopping cart  *  Order    *   Customer   
namespace ThirtyDaysOfTDD
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService _orderService;
        private IOrderDataService _orderDataService;
        private ICustomerService _customerService;
        private IOrderFulfillmentService _orderFulfillmentService;

        [SetUp]
        public void SetupTestFixture()
        {
            _orderDataService = new Mock<IOrderDataService>().Object;
            _customerService = new Mock<ICustomerService>().Object;
            _orderFulfillmentService = new Mock<IOrderFulfillmentService>().Object;
            _orderService = new OrderService(_orderDataService, _customerService, _orderFulfillmentService);
        }

        [Test]
        public void AddOrder_WhenUserPlacesACorrectOrder_AnOrderNumberShouldBeReturned()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 1 });
            var customerId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();
            //var orderService = new OrderService();
            //Mocking
            var orderDataService =new Mock<IOrderDataService>(); // var audioServiceMock = new Mock<IAudioBookService>();
            // 11:Mock.Arrange(() => orderDataService.Save(Arg.IsAny<Order>())).Returns(expectedOrderId);
            orderDataService.Setup(x=> x.Save(It.IsAny<Order>())).Returns(expectedOrderId);

            OrderService orderService = new OrderService(orderDataService.Object, _customerService, _orderFulfillmentService);

            //Act
            var result = orderService.PlaceOrder(customerId, shoppingCart);

            //orderDataService.Verify(x => x.Save(It.Is<Order>(y => y == expectedOrderId)));

            //Assert
            Assert.AreEqual(expectedOrderId, result);
        }

        //When a user places an order for an item and the quantity ordered is zero, then an InvalidOrderException should be thrown.

        [Test]
        public void AddOrder_WhenUserPlacesAZeroQuantityOrder_ABadRequestResultShouldBeReturned()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 0 });
            var customerId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();
            //var orderService = new OrderService();
            
            //Mocking
            var orderDataService = new Mock<IOrderDataService>(); // var audioServiceMock = new Mock<IAudioBookService>();
            // 11:Mock.Arrange(() => orderDataService.Save(Arg.IsAny<Order>())).Returns(expectedOrderId);
            orderDataService.Setup(x => x.Save(It.IsAny<Order>())).Returns(expectedOrderId);

            OrderService orderService = new OrderService(orderDataService.Object, _customerService, _orderFulfillmentService);

            //Act
            var result = orderService.PlaceOrder(customerId, shoppingCart);

            //Assert
            //Assert.AreEqual(expectedOrderId, result);
            Assert.True(result is BadRequestResult);

        }


        [Test]
        public void WhenAValidCustomerPlacesAValidOrderAnOrderShouldBePlaced()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = Guid.NewGuid(), Quantity = 1 });
            var customerId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();

            var customerToReturn = new Customer { Id = customerId, FirstName = "Fred", LastName = "Flinstone" };

            //Mocking
            var orderDataService = new Mock<IOrderDataService>(); // var audioServiceMock = new Mock<IAudioBookService>();
            // 11:Mock.Arrange(() => orderDataService.Save(Arg.IsAny<Order>())).Returns(expectedOrderId);
            orderDataService.Setup(x => x.Save(It.IsAny<Order>())).Returns(expectedOrderId);

            var customerService = new Mock<ICustomerService>();
            customerService.Setup(cusServ => cusServ.GetCustomer(customerId)).Returns(customerToReturn);

            OrderService orderService = new OrderService(orderDataService.Object, customerService.Object, _orderFulfillmentService);
            //_orderService.PlaceOrder(customerId, shoppingCart);

            //Act
            var result = orderService.PlaceOrder(customerId, shoppingCart);

            // audioServiceMock.Verify(x => x.Delete(It.Is<Guid>(guid => guid == id)));
            //assert
            customerService.Verify(x => x.GetCustomer(It.Is<Guid>(y => y == customerId)));
        }

        [Test]
        public void WhenUserPlacesOrderWithItemThatIsInInventoryOrderFulfillmentWorkflowShouldComplete()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            var itemId = Guid.NewGuid();
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = itemId, Quantity = 1 });
            var customerId = Guid.NewGuid();
            var customerToReturn = new Customer { Id = customerId };
            var orderFulfillmentSessionId = Guid.NewGuid();


            //Mock.Arrange(() => _customerService.GetCustomer(customerId)).Returns(customer).OccursOnce();
            var customerService = new Mock<ICustomerService>();
            customerService.Setup(cusServ => cusServ.GetCustomer(customerId)).Returns(customerToReturn);

            customerService.Verify(x => x.GetCustomer(customerId), Times.Once());

            var orderFulfillmentService = new Mock<IOrderFulfillmentService>();
            int callOrder =0;
            orderFulfillmentService.Setup(x=> x.OpenSession(It.IsAny<string>(), It.IsAny<string>()))
                                               .Returns(orderFulfillmentSessionId)
                                               .Callback(() => Assert.That(callOrder++, Is.EqualTo(0)));
            /*Mock.Arrange(() => _orderFulfillmentService.OpenSession(Arg.IsAny<string>(), Arg.IsAny<string>()))
                .Returns(orderFulfillmentSessionId)
                .InOrder();*/



            /*
             Mock.Arrange(() => _orderFulfillmentService.IsInInventory(orderFulfillmentSessionId, itemId, 1))
                .Returns(true)
                .InOrder();
            
            Mock.Arrange(() => 
                _orderFulfillmentService.
                    PlaceOrder(orderFulfillmentSessionId, Arg.IsAny<IDictionary<Guid, int>>(), Arg.IsAny<string>()))
                .Returns(true)
                .InOrder();


            Mock.Arrange(() => _orderFulfillmentService.CloseSession(orderFulfillmentSessionId))
                .InOrder();        
             */

            orderFulfillmentService.Setup(x => x.IsInInventory(orderFulfillmentSessionId,itemId,1))
                                               .Returns(true)
                                               .Callback(() => Assert.That(callOrder++, Is.EqualTo(1)));

            orderFulfillmentService.Setup(x => x.PlaceOrder(orderFulfillmentSessionId,
                It.IsAny<System.Collections.Generic.IDictionary<Guid, int>>()
                , It.IsAny<string>())).Returns(true)
                .Callback(() => Assert.That(callOrder++, Is.EqualTo(2)));


            orderFulfillmentService.Setup(x => x.CloseSession(orderFulfillmentSessionId))
                .Callback(() => Assert.That(callOrder++, Is.EqualTo(3)));

            //Act
            _orderService.PlaceOrder(customerId, shoppingCart);

            //Assert

            ///orderFulfillmentService.Verify(x => x.OpenSession(It.IsAny<string>(), It.IsAny<string>()));

            //Mock.Assert(_orderFulfillmentService);

            Assert.AreEqual(orderFulfillmentSessionId, orderFulfillmentService.Object.OpenSession("foo","voo"));
        }

        [Test]
        public void WhenUserPlacesACorrectOrderWithMoreThenOneItemThenAnOrderNumberShouldBeReturned()
        {
            //Arrange
            var shoppingCart = new ShoppingCart();
            var itemOneId = Guid.NewGuid();
            var itemTwoId = Guid.NewGuid();
            int itemOneQuantity = 1;
            int itemTwoQuantity = 4;
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = itemOneId, Quantity = itemOneQuantity });
            shoppingCart.Items.Add(new ShoppingCartItem { ItemId = itemTwoId, Quantity = itemTwoQuantity });
            var customerId = Guid.NewGuid();
            var expectedOrderId = Guid.NewGuid();
            var orderFulfillmentSessionId = Guid.NewGuid();
            var customer = new Customer { Id = customerId };

            Mock.Arrange(() => _orderDataService.Save(Arg.IsAny<Order>()))
                .Returns(expectedOrderId)
                .OccursOnce();
            Mock.Arrange(() => _customerService.GetCustomer(customerId)).Returns(customer).OccursOnce();
            Mock.Arrange(() => _orderFulfillmentService.OpenSession(Arg.IsAny<string>(), Arg.IsAny<string>()))
                .Returns(orderFulfillmentSessionId);
            Mock.Arrange(() => _orderFulfillmentService.IsInInventory(orderFulfillmentSessionId, itemOneId, itemOneQuantity))
                .Returns(true)
                .OccursOnce();
            Mock.Arrange(() => _orderFulfillmentService.IsInInventory(orderFulfillmentSessionId, itemTwoId, itemTwoQuantity))
                .Returns(true)
                .OccursOnce();
            Mock.Arrange(() =>
                _orderFulfillmentService.
                    PlaceOrder(orderFulfillmentSessionId, Arg.IsAny<IDictionary<Guid, int>>(), Arg.IsAny<string>()))
                .Returns(true);
            //Act
            var result = _orderService.PlaceOrder(customerId, shoppingCart);

            //Assert
            Assert.AreEqual(expectedOrderId, result);
            Mock.Assert(_orderDataService);
            Mock.Assert(_orderFulfillmentService);
        }


    }
}
