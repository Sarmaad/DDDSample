using System.Linq;
using Domain.Infrastructure;
using Domain.Models;

namespace Domain.Specifications
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
