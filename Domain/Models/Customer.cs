using System;

namespace Domain.Models
{
    public class Customer
    {
        public Guid CustomerId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FullName { get { return string.Format("{0} {1}",FirstName,LastName); } }
        public decimal? CustomerCreditLimit { get; private set; }

        internal Customer()
        {
            
        }

        public Customer(Guid id,string firstName, string lastName)
        {
            CustomerId = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public void SetCustomerOrderLimit(decimal? limit)
        {
            CustomerCreditLimit = limit;
        }
    }
}
