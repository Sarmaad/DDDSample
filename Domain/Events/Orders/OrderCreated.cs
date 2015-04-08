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
        public string ShippingAddress1 { get; set; }
        public string ShippingAddress2 { get; set; }
        public string ShippingSuburb { get; set; }
        public string ShippingPostcode { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCountry { get; set; }
    }
}
