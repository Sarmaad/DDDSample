using System.Linq;
using Domain.Infrastructure;
using Domain.Models;

namespace Domain.Specifications
{
    public class CustomerCreditLimitReached:ICustomerCreditLimitReached
    {
        readonly IRepository _repository;
        
        public CustomerCreditLimitReached(IRepository repository)
        {
            _repository = repository;
        }

        public bool IsSatisfiedBy(Order entity)
        {

            // if the customer have overdue order total >= customer limit, then the credit has been reached
            var customer = _repository.Load<Customer>(x => x.CustomerId == entity.CustomerId);

            // no limit was specified, unlimited credit
            if (!customer.CustomerCreditLimit.HasValue) return false;

            // limit was set to zero or less, indicating to stop orders
            if (customer.CustomerCreditLimit.Value <= 0) return true;

            // get all orders that total value >0 and has not been paid fully
            var totalOutstandingOrders = 0m;
                //(from o in _context.Orders
                //                          where o.CustomerId == entity.CustomerId && o.TotalValue > 0 && o.TotalPaid < o.TotalValue
                //                          select new {TotalOutstanding = o.TotalValue - o.TotalPaid}).Sum(x => x.TotalOutstanding) ?? 0;

            // add the current order to totalOutstandingOrders
            if (entity.TotalValue.HasValue && entity.TotalPaid.HasValue)
            {
                totalOutstandingOrders += (entity.TotalValue.Value - entity.TotalPaid.Value);
            }
            

            return totalOutstandingOrders > customer.CustomerCreditLimit;

        }
    }

    public interface ICustomerCreditLimitReached : ISpecification<Order>
    {
    }
}
