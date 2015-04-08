using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using Domain.Exceptions;
using Domain.Spec;
using StructureMap;

namespace Domain.Models
{
    public sealed class Order
    {
        public Guid OrderId { get; private set; }
        public Guid CustomerId { get; private set; }
        public string CustomerFullName { get; private set; }

        public ICollection<OrderLine> OrderLines { get; private set; }
        public ICollection<OrderPayment> OrderPayments { get; private set; }

        public decimal? TotalValue
        {
            get { return OrderLines == null ? (decimal?) null : OrderLines.Sum(x => x.QTY*x.Price); }
            private set { }
        }
        public decimal? TotalPaid
        {
            get { return OrderPayments == null ? (decimal?) null : OrderPayments.Where(x => x.IsSuccessful).Sum(x => x.PaymentAmount); }
            private set { }
        }

        public Address ShippingAddress { get; private set; }
        public OrderStatus OrderStatus { get; private set; }

        internal Order()
        {
            
        }
        public Order(Guid orderId, Guid customerId, string customerFullName, Address shippingAddress)
        {
            OrderId = orderId;
            CustomerId = customerId;
            ShippingAddress = shippingAddress;
            OrderStatus = OrderStatus.Draft;
            CustomerFullName = customerFullName;

        }
        public void ChangeCustomerName(string customerName)
        {
            CustomerFullName = customerName;
        }
        public void AddOrderLine(string productName, int qty, decimal price)
        {
            if (OrderLines==null) OrderLines = new Collection<OrderLine>();
            OrderLines.Add(new OrderLine
                            {
                                ProductName = productName,
                                QTY = qty,
                                Price = price
                            });
        }
        public void ProcessingOrder(ICustomerCreditLimitReached customerCreditLimitReached)
        {
            // make sure the customer is not over the limit
            if (customerCreditLimitReached.IsSatisfiedBy(this))
                throw new CustomerLimitReachedException(CustomerId, OrderId, TotalValue.HasValue ? TotalValue.Value : 0);

            OrderStatus = OrderStatus.Processing;
        }
        public void AddPayment(DateTime paidOn,decimal amount,bool isSuccessful)
        {
            if(OrderStatus==OrderStatus.Draft) throw new Exception("Order is in Draft!");
            if (OrderPayments == null) OrderPayments = new Collection<OrderPayment>();
            OrderPayments.Add(new OrderPayment
                               {
                                   PaymentDate = paidOn,
                                   PaymentAmount = amount,
                                   IsSuccessful = isSuccessful
                               });
        }
    }

    public enum OrderStatus
    {
        Draft,
        Processing,
        Shipped,
        Complete
    }

    public sealed class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Suburb { get; set; }
        public string Postcode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

    public class OrderLine
    {
        public int OrderLineId { get; set; }
        public string ProductName { get; set; }
        public int QTY { get; set; }
        public decimal Price { get; set; }
    }
    public class OrderPayment
    {
        public int OrderPaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsSuccessful { set; get; }
    }

    
}
