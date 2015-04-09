using System;
using System.Linq;
using Domain.Infrastructure;

namespace Domain.Query
{
    public sealed class CustomerOrders : IQuery<CustomerOrders.Result>
    {
        readonly Guid _customerId;

        public class Result
        {
            public Guid OrderId;
            public decimal TotalValue;
            public decimal TotalPaid;
            public decimal TotalOutstanding;
        }

        public CustomerOrders(Guid customerId)
        {
            _customerId = customerId;
        }

        public QueryResult<Result> Execute(IAppContext context)
        {
            var result = new QueryResult<Result>();

            var q = context.Orders.Where(x => x.CustomerId == _customerId);

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