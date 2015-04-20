using System;
using Domain.Exceptions;
using Domain.Infrastructure;
using Domain.Models;
using Domain.Specifications;
using Messages.Commands;
using Messages.Events;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;

namespace Handlers.Tests
{
    [TestFixture]
    public class CustomerHandlerTest:NSBBaseTest
    {
        [Test]
        public void CreateCustomer_Basic()
        {
            var cmd = new CreateCustomer
                      {
                          CustomerId = Guid.NewGuid(),
                          FirstName = "John",
                          LastName = "Smith",
                          Email = "john.smith@domain.com"
                      };

            var repository = new Mock<IRepository>();
            var specificationFactory = new Mock<ISpecificationFactory>();
            specificationFactory.Setup(x => x.Get<IDuplicateCustomerEmail>())
                                .Returns(() =>
                                {
                                    var dup = new Mock<IDuplicateCustomerEmail>();
                                    dup.Setup(x => x.IsSatisfiedBy(It.IsAny<Customer>())).Returns(false);
                                    return dup.Object;
                                });

            Test.Handler(bus => new CustomerHandler(repository.Object, bus,specificationFactory.Object))
                .ExpectPublish<ICustomerCreated>(e => e.CustomerId == cmd.CustomerId && e.Email == cmd.Email)
                .OnMessage(cmd);
        }

        [Test]
        [ExpectedException(typeof(DuplicateEmailException))]
        public void CreateCustomer_DuplicateEmail()
        {
            var cmd = new CreateCustomer
            {
                CustomerId = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Smith",
                Email = "john.smith@domain.com"
            };

            var repository = new Mock<IRepository>();
            var specificationFactory = new Mock<ISpecificationFactory>();
            specificationFactory.Setup(x => x.Get<IDuplicateCustomerEmail>())
                                .Returns(() =>
                                         {
                                             var dup = new Mock<IDuplicateCustomerEmail>();
                                             dup.Setup(x => x.IsSatisfiedBy(It.IsAny<Customer>())).Returns(true);
                                             return dup.Object;
                                         });

            Test.Handler(bus => new CustomerHandler(repository.Object, bus, specificationFactory.Object))
                .ExpectNotPublish<ICustomerCreated>(e => e.CustomerId == cmd.CustomerId && e.Email == cmd.Email)
                .OnMessage(cmd);
        }
    }
}
