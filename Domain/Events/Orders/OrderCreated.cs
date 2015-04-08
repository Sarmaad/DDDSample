using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Events.Orders
{
    public class OrderCreated
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerFullName { get; set; }
        public string OrderStatus { get; set; }
    }
}
