using System;
using System.Collections.Generic;
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
            
            Context.Orders.Add(domain);
            Context.SaveChanges();

            var order = Context.Orders.Single(x => x.OrderId == domain.OrderId);

            Assert.NotNull(order);
            Assert.AreEqual(customerId, order.CustomerId);
            Assert.AreEqual(customerFullName, order.CustomerFullName);
        }

        [Test,ExpectedException(typeof(CustomerLimitReachedException))]
        public void CreateOrder_UnableToProcessOverlimitCustomer()
        {
            var customerCreditLimitReached = new Mock<ICustomerCreditLimitReached>();
            customerCreditLimitReached.Setup(x => x.IsSatisfiedBy(It.IsAny<Order>())).Returns(true);
            
            var domain = DefaultOrder();
            domain.ProcessingOrder(customerCreditLimitReached.Object);
        }

        [Test, ExpectedException(typeof(EntityNotFoundException))]
        public void CreateOrder_CustomerDoesNotExists()
        {
            var customerExistsSpecification = new Mock<ICustomerExistsSpecification>();
            customerExistsSpecification.Setup(x => x.IsSatisfiedBy(It.IsAny<Order>())).Returns(true);

            var domain = DefaultOrder(customerExistsSpecification: customerExistsSpecification.Object);


            Context.Orders.Add(domain);
            Context.SaveChanges();

        }



        public static Order DefaultOrder(Guid? customerId = null, string fullName=null, Guid? orderId = null, ICustomerExistsSpecification customerExistsSpecification=null)
        {
            if (!customerId.HasValue) customerId = Guid.NewGuid();
            if (string.IsNullOrWhiteSpace(fullName)) fullName = "Test Customer";
            if (!orderId.HasValue) orderId = Guid.NewGuid();
            if (customerExistsSpecification == null)
            {
                var mCustomerExistsSpecification = new Mock<ICustomerExistsSpecification>();
                mCustomerExistsSpecification.Setup(x => x.IsSatisfiedBy(It.IsAny<Order>())).Returns(false);
                customerExistsSpecification = mCustomerExistsSpecification.Object;
            }

            return new Order(orderId.Value, customerId.Value,fullName,customerExistsSpecification);
        }
    }
}
