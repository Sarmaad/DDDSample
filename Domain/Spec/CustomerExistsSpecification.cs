using System;
using System.Linq;
using Domain.Infrastructure;
using Domain.Models;

namespace Domain.Spec
{
    public class CustomerExistsSpecification : ICustomerExistsSpecification
    {
        readonly IAppContext _context;

        public CustomerExistsSpecification(IAppContext context)
        {
            _context = context;
        }

        public bool IsSatisfiedBy(Order entity)
        {
            return _context.Customers.Any(x => x.CustomerId == entity.CustomerId); 
        }
    }

    public interface ICustomerExistsSpecification : ISpecification<Order>
    {
    }
}
