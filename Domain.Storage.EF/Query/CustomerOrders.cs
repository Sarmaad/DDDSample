using System;
using System.Linq;
using Domain.Infrastructure;
using Domain.Infrastructure.Interfaces;

namespace Domain.Storage.EF.Query
{
    // Query input and result DTOs
    public sealed class CustomerOrders : IQuery<CustomerOrders.Result>
    {
        public Guid? CustomerId{get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public class Result
        {
            public Guid OrderId;
            public decimal TotalValue;
            public decimal TotalPaid;
            public decimal TotalOutstanding;
        }
    }

    // Query logic processor
    public class CustomerOrdersQueryHandler:IQueryHandler<CustomerOrders,CustomerOrders.Result>
    {
        readonly IAppContext _context;

        public CustomerOrdersQueryHandler(IAppContext context)
        {
            _context = context;
        }

        public QueryResult<CustomerOrders.Result> Handle(CustomerOrders query)
        {
            var result = new QueryResult<CustomerOrders.Result>();

            var q = _context.Orders.AsQueryable();

            if (query.CustomerId.HasValue)
                q = q.Where(x => x.CustomerId == query.CustomerId.Value);

            if (!string.IsNullOrWhiteSpace(query.FirstName))
                q = q.Where(x => x.CustomerFullName.StartsWith(query.FirstName, StringComparison.InvariantCultureIgnoreCase));

            if (!string.IsNullOrWhiteSpace(query.LastName))
                q = q.Where(x => x.CustomerFullName.EndsWith(query.LastName, StringComparison.InvariantCultureIgnoreCase));

            result.TotalFound = q.Count();
            result.Results = (from x in q
                              let totalValue = x.TotalValue ?? 0
                              let totalPaid = x.TotalPaid ?? 0
                              let totalOutstanding = totalValue - totalPaid
                              select new CustomerOrders.Result
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