using System;

namespace Messages.Commands
{
    public class CreateOrder
    {
        public Guid CustomerId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid OrderId { get; set; }
        // shipping address
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }

    }
}