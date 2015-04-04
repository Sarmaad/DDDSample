using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using NUnit.Framework;

namespace Domain.Test.Integration
{
    [TestFixture]
    public class CustomerTests:SqlCeBaseTest
    {
        
        [SetUp]
        public void Setup()
        {
            
        }

        [TearDown]
        public void TearDown()
        {
            
        }

        [Test]
        public void CreateCustomer()
        {
            var domain = DefaulCustomer();
            Context.Customers.Add(domain);
            Context.SaveChanges();

            var customer = Context.Customers.Single(x => x.CustomerId == domain.CustomerId);

            Assert.NotNull(customer);
        }

        [Test]
        public void CreateCustomer_WithLimits()
        {
            var limit = 10.95m;
            var domain = DefaulCustomer();
            domain.SetCustomerOrderLimit(limit);
            
            Context.Customers.Add(domain);
            Context.SaveChanges();

            var customer = Context.Customers.Single(x => x.CustomerId == domain.CustomerId);

            Assert.NotNull(customer);
            Assert.AreEqual(limit, customer.CustomerCreditLimit);
        }

        public static Customer DefaulCustomer(Guid? id = null)
        {
            if (!id.HasValue) id = Guid.NewGuid();

            return new Customer(id.Value, "Sarmaad", "Amin");
        }
    }
}
