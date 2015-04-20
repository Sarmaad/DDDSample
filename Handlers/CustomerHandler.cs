using Domain.Infrastructure;
using Domain.Models;
using Domain.Specifications;
using Messages.Commands;
using Messages.Events;
using NServiceBus;

namespace Handlers
{
    public class CustomerHandler:IHandleMessages<CreateCustomer>
    {
        readonly IRepository _repository;
        readonly IBus _bus;
        readonly IDuplicateCustomerEmail _duplicateCustomerEmail;

        public CustomerHandler(IRepository repository, IBus bus, IDuplicateCustomerEmail duplicateCustomerEmail)
        {
            _repository = repository;
            _bus = bus;
            _duplicateCustomerEmail = duplicateCustomerEmail;
        }

        public void Handle(CreateCustomer message)
        {
            var customer = new Customer(message.CustomerId, message.FirstName, message.LastName, message.Email,_duplicateCustomerEmail);

            _repository.Add(customer);

            _bus.Publish<ICustomerCreated>(e =>
                                           {
                                               e.CustomerId = customer.CustomerId;
                                               e.FirstName = customer.FirstName;
                                               e.LastName = customer.LastName;
                                               e.Email = customer.Email;

                                           });
        }
    }
}
