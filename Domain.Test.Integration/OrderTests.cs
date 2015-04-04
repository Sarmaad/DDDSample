using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        [Test]
        public void CreateOrder_OverlimitCustomer()
        {
            var customer = CustomerTests.DefaulCustomer();
            Context.Customers.Add(customer);
            Context.SaveChanges();

            var domain = DefaultOrder(Context, customer.CustomerId, customer.FullName);

            Context.Orders.Add(domain);
            Context.SaveChanges();

            var order = Context.Orders.Single(x => x.OrderId == domain.OrderId);

            Assert.NotNull(order);
            Assert.AreEqual(customer.CustomerId, order.CustomerId);
            Assert.AreEqual(customer.FullName, order.CustomerFullName);
        }



        public static Order DefaultOrder(IAppContext context, Guid customerId, string fullName, Guid? orderId = null)
        {
            if (!orderId.HasValue) orderId = Guid.NewGuid();

            return new Order(orderId.Value, customerId,fullName, context);
        }
    }
}
