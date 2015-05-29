using System;
using System.Linq;
using Domain.Infrastructure;

namespace Domain.Query
{
    public sealed class CustomerOrders : IQuery<CustomerOrders.Result>
    {
        readonly Guid? _customerId;
        private readonly string _firstName;
        private readonly string _lastName;

        public class Result
        {
            public Guid OrderId;
            public decimal TotalValue;
            public decimal TotalPaid;
            public decimal TotalOutstanding;
        }

        public CustomerOrders(Guid? customerId = null, string firstName=null, string lastName=null)
        {
            _customerId = customerId;
            _firstName = firstName;
            _lastName = lastName;
        }

        public QueryResult<Result> Execute(IAppContext context)
        {
            var result = new QueryResult<Result>();

            var q = context.Orders.AsQueryable();

            if (_customerId.HasValue)
                q = q.Where(x => x.CustomerId == _customerId);

            if (!string.IsNullOrWhiteSpace(_firstName))
                q = q.Where(x => x.CustomerFullName.StartsWith(_firstName,StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(_lastName))
                q = q.Where(x => x.CustomerFullName.EndsWith(_lastName, StringComparison.InvariantCultureIgnoreCase));

            result.TotalFound = q.Count();
            result.Results = (from x in q
                              let totalValue = x.TotalValue.HasValue ? x.TotalValue.Value : 0
                              let totalPaid = x.TotalPaid.HasValue ? x.TotalPaid.Value : 0
                              let totalOutstanding = totalValue - totalPaid
                              select new Result
                                     {
                                         OrderId = x.OrderId,
                                         TotalValue = totalValue,
                                         TotalPaid = totalPaid,
                                         TotalOutstanding = totalOutstanding
                                     }).ToArray();
                

            return result;
        }
    }
}