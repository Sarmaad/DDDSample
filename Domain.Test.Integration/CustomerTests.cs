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
    public class CustomerTests:SqlCeBaseTest
    {
        Mock<IDuplicateCustomerEmail> _duplicateCustomerEmail;

        [SetUp]
        public void Setup()
        {
            _duplicateCustomerEmail = new Mock<IDuplicateCustomerEmail>();
            _duplicateCustomerEmail.Setup(x => x.IsSatisfiedBy(It.IsAny<Customer>())).Returns(false);
        }


        [Test]
        public void CreateCustomer()
        {
            var domain = DefaulCustomer(_duplicateCustomerEmail.Object);
            Context.Customers.Add(domain);
            Context.SaveChanges();

            var customer = Context.Customers.Single(x => x.CustomerId == domain.CustomerId);

            Assert.NotNull(customer);
        }

        [Test]
        public void CreateCustomer_WithLimits()
        {
            
            var limit = 10.95m;
            var domain = DefaulCustomer(_duplicateCustomerEmail.Object);
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
            _duplicateCustomerEmail.Setup(x => x.IsSatisfiedBy(It.IsAny<Customer>())).Returns(true);
            Context.Customers.Add(DefaulCustomer(_duplicateCustomerEmail.Object));
            Context.SaveChanges();
        }

        public static Customer DefaulCustomer(IDuplicateCustomerEmail duplicateCustomerEmail, Guid? id = null)
        {
            if (!id.HasValue) id = Guid.NewGuid();

            return new Customer(id.Value, "Sarmaad", "Amin", "sarmaad@gmail.com", duplicateCustomerEmail);
        }
    }
}
