using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Infrastructure;
using Domain.Models;
using Domain.Spec;
using Moq;
using NUnit.Framework;


namespace Domain.Test.Integration
{
    [TestFixture]
    public class OrderTests:SqlCeBaseTest
    {
        Mock<IDuplicateCustomerEmail> _duplicateCustomerEmail;

        [SetUp]
        public void Setup()
        {
            _duplicateCustomerEmail = new Mock<IDuplicateCustomerEmail>();
        }
        
        [Test]
        public void CreateOrder()
        {
            var customerId = Guid.NewGuid();
            var customerFullName = "Test Customer";
            var domain = DefaultOrder(customerId, customerFullName);

            domain.AddOrderLine("Test Product", 1, 10.95m);

            Context.Orders.Add(domain);
            Context.SaveChanges();

            Read(context =>
                 {
                     var order = context.Orders.Include(x => x.OrderLines).Single(x => x.OrderId == domain.OrderId);

                     Assert.NotNull(order);
                     Assert.AreEqual(customerId, order.CustomerId);
                     Assert.AreEqual(customerFullName, order.CustomerFullName);
                     Assert.AreEqual(1, order.OrderLines.Count);
                 });
        }

        [Test,ExpectedException(typeof(CustomerLimitReachedException))]
        public void CreateOrder_UnableToProcessOverlimitCustomer()
        {
            var customerCreditLimitReached = new Mock<ICustomerCreditLimitReached>();
            customerCreditLimitReached.Setup(x => x.IsSatisfiedBy(It.IsAny<Order>())).Returns(true);
            
            var domain = DefaultOrder();
            domain.ProcessingOrder(customerCreditLimitReached.Object);
        }

        [Test]
        public void CreateOrder_PaidInFull()
        {
            var customerCreditLimitReached = new Mock<ICustomerCreditLimitReached>();
            customerCreditLimitReached.Setup(x => x.IsSatisfiedBy(It.IsAny<Order>())).Returns(false);

            var orderValue = 11.95m;

            var domain = DefaultOrder();
            domain.AddOrderLine("Test Product", 1, orderValue);
            domain.ProcessingOrder(customerCreditLimitReached.Object);
            domain.AddPayment(DateTime.Now, orderValue, true);

            Context.Orders.Add(domain);
            Context.SaveChanges();


            Read(context =>
                 {
                     var order = context.Orders.Include(x => x.OrderLines).Include(x => x.OrderPayments).Single(x => x.OrderId == domain.OrderId);

                     Assert.AreEqual(orderValue, order.TotalValue);
                     Assert.AreEqual(orderValue, order.TotalPaid);
                 });




        }
        



        public static Order DefaultOrder(Guid? customerId = null, string fullName=null, Guid? orderId = null, Address shippingAddress=null)
        {
            if (!customerId.HasValue) customerId = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = "Test Customer";
            if (!orderId.HasValue) orderId = Guid.NewGuid();
            if(shippingAddress==null)shippingAddress = new Address();

            return new Order(orderId.Value, customerId.Value, fullName, shippingAddress);
        }
    }
}
