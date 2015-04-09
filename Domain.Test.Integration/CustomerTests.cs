using System;
using System.Linq;
using Domain.Exceptions;
using Domain.Models;
using Domain.Specifications;
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
            Repository(repository => repository.Add(domain));

            Repository(repository=>
                       {
                           var customer = repository.Load<Customer>(x => x.CustomerId == domain.CustomerId);
                     //var customer = context.Customers.Single(x => x.CustomerId == domain.CustomerId);

                     Assert.NotNull(customer);
                 });
        }

        [Test]
        public void CreateCustomer_WithLimits()
        {
            
            var limit = 10.95m;
            var domain = DefaulCustomer(_duplicateCustomerEmail.Object);
            domain.SetCustomerOrderLimit(limit);

            Repository(repository => repository.Add(domain));

            Repository(repository =>
                       {
                           var customer = repository.Load<Customer>(x => x.CustomerId == domain.CustomerId);
                     //var customer = context.Customers.Single(x => x.CustomerId == domain.CustomerId);

                     Assert.NotNull(customer);
                     Assert.AreEqual(limit, customer.CustomerCreditLimit);
                 });
        }

        [Test, ExpectedException(typeof(DuplicateEmailException))]
        public void CreateCustomer_DuplciateEmailAddress()
        {
            _duplicateCustomerEmail.Setup(x => x.IsSatisfiedBy(It.IsAny<Customer>())).Returns(true);
            var domain = DefaulCustomer(_duplicateCustomerEmail.Object);
            Repository(repository => repository.Add(domain));

        }

        public static Customer DefaulCustomer(IDuplicateCustomerEmail duplicateCustomerEmail, Guid? id = null)
        {
            if (!id.HasValue) id = Guid.NewGuid();

            return new Customer(id.Value, "Sarmaad", "Amin", "sarmaad@gmail.com", duplicateCustomerEmail);
        }
    }
}
