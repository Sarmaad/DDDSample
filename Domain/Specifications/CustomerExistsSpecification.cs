using System.Linq;
using Domain.Infrastructure;
using Domain.Infrastructure.Interfaces;
using Domain.Models;

namespace Domain.Specifications
{
    public class CustomerExistsSpecification : ICustomerExistsSpecification
    {
        readonly IRepository _repository;
        

        public CustomerExistsSpecification(IRepository repository)
        {
            _repository = repository;
            
        }

        public bool IsSatisfiedBy(Order entity)
        {
            return _repository.Project<Customer, bool>(customers => customers.Any(x => x.CustomerId == entity.CustomerId));
        }
    }

    public interface ICustomerExistsSpecification : ISpecification<Order>
    {
    }
}
