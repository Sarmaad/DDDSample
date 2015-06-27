using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Infrastructure;
using Domain.Infrastructure.Interfaces;
using Domain.Models;
using Messages.Commands;
using Messages.Events;
using Ninject;
using NServiceBus;
using NServiceBus.Saga;

namespace Handlers
{
    public class OrderProcessor:Saga<OrderProcessorState>,IAmStartedByMessages<CreateOrder>, IHandleMessages<ICustomerCreated>
    {
        [Inject]
        public IRepository Repository { private get; set; }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<OrderProcessorState> mapper)
        {
            mapper.ConfigureMapping<CreateOrder>(m=>m.OrderId).ToSaga(s=>s.OrderId);
            mapper.ConfigureMapping<ICustomerCreated>(m=>m.CustomerId).ToSaga(s=>s.CustomerId);
        }

        public void Handle(CreateOrder message)
        {
            Data.CustomerId = message.CustomerId;
            Data.Email = message.Email;
            Data.FirstName = message.FirstName;
            Data.LastName = message.LastName;
            Data.OrderId = message.OrderId;
            

            // ensure the customer exists, if not, create it
            var customer = Repository.Load<Customer>(x => x.CustomerId == message.CustomerId);
            if (customer == null)
            {
                Bus.Send(new CreateCustomer
                         {
                             CustomerId = Data.CustomerId,
                             Email = Data.Email,
                             FirstName = Data.FirstName,
                             LastName = Data.LastName

                         });
                return;
            }

            CreateOrder();
            EndOrderProcess();
        }
        public void Handle(ICustomerCreated message)
        {
            CreateOrder();
            EndOrderProcess();
        }

        void CreateOrder()
        {
            var order = new Domain.Models.Order(Data.OrderId, Data.CustomerId, string.Format("{0} {1}", Data.FirstName, Data.LastName),
                                                Data.Address1, Data.Address2, Data.Suburb, Data.State, Data.Postcode, Data.Country);
            Repository.Add(order);
        }

        void EndOrderProcess()
        {
            Bus.Publish<IOrderCreated>(e =>
                                       {
                                           e.OrderId = Data.OrderId;
                                       });

            MarkAsComplete();
        }
    }

    public class OrderProcessorState : IContainSagaData
    {
        public virtual Guid Id { get; set; }
        public virtual string Originator { get; set; }
        public virtual string OriginalMessageId { get; set; }

        public virtual Guid CustomerId { get; set; }
        public virtual string Email { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        
        public virtual Guid OrderId { get; set; }
        public virtual string Address1 { get; set; }
        public virtual string Address2 { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string State { get; set; }
        public virtual string Postcode { get; set; }
        public virtual string Country { get; set; }
    }
}
