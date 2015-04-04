using System;
using System.Linq;
using Domain.Infrastructure;
using Domain.Models;

namespace Domain.Spec
{
    public class CustomerExistsSpecification:ISpecification<Order>
    {
        readonly IAppContext _context;

        public CustomerExistsSpecification(IAppContext context)
        {
            _context = context;
        }

        public bool IsSatisfiedBy(Order entity)
        {

            var found=_context.Customers.Any(x => x.CustomerId == entity.CustomerId);

            return found;


        }
    }
}
