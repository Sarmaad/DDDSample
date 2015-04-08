using System;

namespace Domain.Events.Orders
{
    public class PaymentAdded
    {
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsSuccessful { get; set; }
    }
}