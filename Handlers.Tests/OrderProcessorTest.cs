using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class OrderProcessorTest:NSBBaseTest
    {
        [Test]
        public void CreateOrder_NewCustomer()
        {
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var repository = new Mock<IRepository>();
            repository.Setup(x => x.Add(It.IsAny<Order>()))
                      .Callback<Order>(order =>
                                       {
                                           Assert.AreEqual(orderId, order.OrderId);
                                       });

            Test.Saga<OrderProcessor>()
                .WithExternalDependencies(processor => processor.Repository = repository.Object)
                .WhenHandling<CreateOrder>(c =>
                                           {
                                               c.CustomerId = customerId;
                                               c.Email = "john.smith@domain.com";
                                               c.FirstName = "John";
                                               c.LastName = "Smith";
                                               c.OrderId = orderId;
                                               c.Address1 = "132 George St";
                                               c.Suburb = "Sydney";
                                               c.Postcode = "2001";
                                               c.State = "NSW";
                                               c.Country = "Australia";
                                           })
                .WhenHandling<ICustomerCreated>(e =>
                                                {
                                                    e.CustomerId = customerId;
                                                    e.Email = "johnsmith@domain.com";

                                                })
                .ExpectSend<CreateCustomer>(customer => customer!=null)
                .ExpectPublish<IOrderCreated>()
                .AssertSagaCompletionIs(true)
                ;
        }

        [Test]
        public void CreateOrder_ExistingCustomer()
        {
            var orderId = Guid.NewGuid();
            var customerId = Guid.NewGuid();

            var repository = new Mock<IRepository>();
            repository.Setup(x => x.Load(It.IsAny<Func<Customer, bool>>())).Returns(new Customer(customerId,"John","Smith","john.smith@domain.com",new Mock<IDuplicateCustomerEmail>().Object));
            repository.Setup(x => x.Add(It.IsAny<Order>())) 
                      .Callback<Order>(order =>
                      {
                          Assert.AreEqual(orderId, order.OrderId);
                      });

            Test.Saga<OrderProcessor>()
                .WithExternalDependencies(processor => processor.Repository = repository.Object)
                .WhenHandling<CreateOrder>(c =>
                {
                    c.CustomerId = customerId;
                    c.Email = "john.smith@domain.com";
                    c.FirstName = "John";
                    c.LastName = "Smith";
                    c.OrderId = orderId;
                    c.Address1 = "132 George St";
                    c.Suburb = "Sydney";
                    c.Postcode = "2001";
                    c.State = "NSW";
                    c.Country = "Australia";
                })
                .WhenHandling<ICustomerCreated>()
                .ExpectNotSend<CreateCustomer>(customer => customer==null)
                .ExpectPublish<IOrderCreated>()
                .AssertSagaCompletionIs(true)
                ;


        }
    }
}
