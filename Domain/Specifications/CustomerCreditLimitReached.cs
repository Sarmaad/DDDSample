using System.Linq;
using Domain.Infrastructure;
using Domain.Infrastructure.Interfaces;
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
            
            var or = _repository.Project<Order, decimal?>(
                orders =>
                    (from o in orders
                        where o.CustomerId == entity.CustomerId && o.TotalValue > 0 && (o.TotalPaid < o.TotalValue || !o.TotalPaid.HasValue)
                        let totalValue = o.TotalValue ?? 0m
                        let totalPaid = o.TotalPaid ?? 0m
                        let totalOutstanding = totalValue - totalPaid
                        select totalOutstanding).Sum());
            
            // add the current order to totalOutstandingOrders
            if (entity.TotalValue.HasValue)
            {
                totalOutstandingOrders += (entity.TotalValue.Value - (entity.TotalPaid.HasValue ? entity.TotalPaid.Value : 0));
            }
            

            return totalOutstandingOrders > customer.CustomerCreditLimit;

        }
    }

    public interface ICustomerCreditLimitReached : ISpecification<Order>
    {
    }
}
