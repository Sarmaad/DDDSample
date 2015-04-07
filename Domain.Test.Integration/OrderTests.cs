using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Models;
using NUnit.Framework;


namespace Domain.Test.Integration
{
    [TestFixture]
    public class OrderTests:SqlCeBaseTest
    {
        [Test]
        public void CreateOrder()
        {
            var customer = CustomerTests.DefaulCustomer();
            Context.Customers.Add(customer);
            Context.SaveChanges();

            var domain = DefaultOrder(Context, customer.CustomerId, customer.FullName);
            
            Context.Orders.Add(domain);
            Context.SaveChanges();

            var order = Context.Orders.Single(x => x.OrderId == domain.OrderId);

            Assert.NotNull(order);
            Assert.AreEqual(customer.CustomerId,order.CustomerId);
            Assert.AreEqual(customer.FullName,order.CustomerFullName);
        }

        [Test,ExpectedException(typeof(CustomerLimitReachedException))]
        public void CreateOrder_UnableToProcessOverlimitCustomer()
        {
            var customer = CustomerTests.DefaulCustomer();
            customer.SetCustomerOrderLimit(10);
            Context.Customers.Add(customer);
            Context.SaveChanges();

            var domain = DefaultOrder(Context, customer.CustomerId, customer.FullName);
            domain.AddOrderLine("Test product",10,10.95m);
            domain.ProcessingOrder(Context);
           

        }

        [Test, ExpectedException(typeof(EntityNotFoundException))]
        public void CreateOrder_CustomerDoesNotExists()
        {
           var domain = DefaultOrder(Context, Guid.NewGuid(), "John Smith");


            Context.Orders.Add(domain);
            Context.SaveChanges();

        }



        public static Order DefaultOrder(IAppContext context, Guid customerId, string fullName, Guid? orderId = null)
        {
            if (!orderId.HasValue) orderId = Guid.NewGuid();

            return new Order(orderId.Value, customerId,fullName, context);
        }
    }
}
