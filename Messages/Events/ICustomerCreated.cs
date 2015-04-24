using System;

namespace Messages.Events
{
    public interface ICustomerCreated
    {
        Guid CustomerId { get; set; }
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
    }
}
