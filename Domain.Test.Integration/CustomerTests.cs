using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
using Domain.Models;
using NUnit.Framework;

namespace Domain.Test.Integration
{
    [TestFixture]
    public class CustomerTests:SqlCeBaseTest
    {
        
        [Test]
        public void CreateCustomer()
        {
            var domain = DefaulCustomer(Context);
            Context.Customers.Add(domain);
            Context.SaveChanges();

            var customer = Context.Customers.Single(x => x.CustomerId == domain.CustomerId);

            Assert.NotNull(customer);
        }

        [Test]
        public void CreateCustomer_WithLimits()
        {
            var limit = 10.95m;
            var domain = DefaulCustomer(Context);
            domain.SetCustomerOrderLimit(limit);
            
            Context.Customers.Add(domain);
            Context.SaveChanges();

            var customer = Context.Customers.Single(x => x.CustomerId == domain.CustomerId);

            Assert.NotNull(customer);
            Assert.AreEqual(limit, customer.CustomerCreditLimit);
        }

        [Test, ExpectedException(typeof(DuplicateEmailException))]
        public void CreateCustomer_DuplciateEmailAddress()
        {
            // add the first customer
            Context.Customers.Add(DefaulCustomer(Context));
            Context.SaveChanges();

            // add the second customer
            Context.Customers.Add(DefaulCustomer(Context));
            Context.SaveChanges();
            
        }

        public static Customer DefaulCustomer(IAppContext context,Guid? id = null)
        {
            if (!id.HasValue) id = Guid.NewGuid();

            return new Customer(id.Value, "Sarmaad", "Amin", "sarmaad@gmail.com",context);
        }
    }
}
