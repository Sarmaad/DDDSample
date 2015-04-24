using System;

namespace Messages.Commands
{
    public class CreateCustomer
    {
        public Guid CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
