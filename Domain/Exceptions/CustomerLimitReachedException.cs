using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class CustomerLimitReachedException:Exception
    {
        public Guid CustomerId { get; private set; }
        public Guid OrderId { get; private set; }
        public decimal OrderValue { get; private set; }

        public CustomerLimitReachedException(Guid customerId,Guid orderId, decimal orderValue)
        {
            CustomerId = customerId;
            OrderId = orderId;
            OrderValue = orderValue;
        }
    }
}
