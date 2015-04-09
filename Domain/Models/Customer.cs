using System;
using Domain.Exceptions;
using Domain.Infrastructure;
using Domain.Specifications;

namespace Domain.Models
{
    public sealed class Customer
    {
        public Guid CustomerId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string FullName { get { return string.Format("{0} {1}",FirstName,LastName); } }
        public decimal? CustomerCreditLimit { get; private set; }

        internal Customer(){}
        public Customer(Guid id, string firstName, string lastName, string email, IDuplicateCustomerEmail duplicateCustomerEmail)
        {
            CustomerId = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;

            if (duplicateCustomerEmail.IsSatisfiedBy(this))
                throw new DuplicateEmailException(email);
        }

        
        public void SetCustomerOrderLimit(decimal? limit)
        {
            CustomerCreditLimit = limit;
        }
    }

    
}
