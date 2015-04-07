using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Infrastructure;
using Domain.Models;

namespace Domain.Spec
{
    public class DuplicateCustomerEmail : IDuplicateCustomerEmail
    {
        readonly IAppContext _context;

        public DuplicateCustomerEmail(IAppContext context)
        {
            _context = context;
        }

        public bool IsSatisfiedBy(Customer entity)
        {
            // customer must have a unique email address
            return _context.Customers.Any(x => x.Email == entity.Email);
        }
    }

    public interface IDuplicateCustomerEmail : ISpecification<Customer>
    {
    }
}
