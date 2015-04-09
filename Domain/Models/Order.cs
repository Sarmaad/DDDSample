using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Domain.Events.Orders;
using Domain.Exceptions;
using Domain.Infrastructure;
using Domain.Specifications;

namespace Domain.Models
{
    /// <summary>
    /// Order Domain implement CQRS by inheriting from AggregateBase
    /// </summary>
    public sealed class Order:AggregateBase
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
        public Order(Guid orderId, Guid customerId, string customerFullName, string address1, string address2, string suburb, string state, string postcode, string country)
        {
            RaiseEvent(new OrderCreated
                  {
                      OrderId=orderId,
                      CustomerId = customerId,
                      CustomerFullName = customerFullName,
                      OrderStatus= OrderStatus.Draft.ToString(),
                      ShippingAddress1 = address1,
                      ShippingAddress2 = address2,
                      ShippingSuburb = suburb,
                      ShippingPostcode = postcode,
                      ShippingState = state,
                      ShippingCountry = country,
                  });
            
        }
        public void ChangeCustomerName(string customerName)
        {
            RaiseEvent(new CustomerNameChanged {CustomerName = customerName});
        }
        public void AddOrderLine(string productName, int qty, decimal price)
        {
            RaiseEvent(new OrderLineAdded
            {
                ProductName=productName,
                QTY = qty,
                Price = price
            });

           
        }
        public void ProcessingOrder(ICustomerCreditLimitReached customerCreditLimitReached)
        {
            // make sure the customer is not over the limit
            if (customerCreditLimitReached.IsSatisfiedBy(this))
                throw new CustomerLimitReachedException(CustomerId, OrderId, TotalValue.HasValue ? TotalValue.Value : 0);

            RaiseEvent(new OrderProcessed{OrderStatus = Models.OrderStatus.Processing.ToString()});
        }
        public void AddPayment(DateTime paidOn,decimal amount,bool isSuccessful)
        {
            if (OrderStatus == OrderStatus.Draft) throw new Exception("Order is in Draft!");

            RaiseEvent(new PaymentAdded
            {
                PaymentDate = paidOn,
                PaymentAmount = amount,
                IsSuccessful = isSuccessful
            });
        }



        void When(OrderCreated e)
        {
            OrderId = e.OrderId;
            CustomerId = e.CustomerId;

            OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), e.OrderStatus);
            CustomerFullName = e.CustomerFullName;
            ShippingAddress = new Address
            {
                Address1 = e.ShippingAddress1,
                Address2 = e.ShippingAddress2,
                Suburb = e.ShippingSuburb,
                Postcode = e.ShippingPostcode,
                State = e.ShippingState,
                Country = e.ShippingCountry

            };
        }
        void When(CustomerNameChanged e)
        {
            CustomerFullName = e.CustomerName;
        }
        void When(OrderLineAdded e)
        {
            if (OrderLines == null) OrderLines = new Collection<OrderLine>();
            OrderLines.Add(new OrderLine
            {
                ProductName = e.ProductName,
                QTY = e.QTY,
                Price = e.Price
            });
        }
        void When(OrderProcessed e)
        {
            OrderStatus = (OrderStatus) Enum.Parse(typeof (OrderStatus), e.OrderStatus);
        }
        void When(PaymentAdded e)
        {
            if (OrderPayments == null) OrderPayments = new Collection<OrderPayment>();
            OrderPayments.Add(new OrderPayment
            {
                PaymentDate = e.PaymentDate,
                PaymentAmount = e.PaymentAmount,
                IsSuccessful = e.IsSuccessful
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
        public Guid OrderId { get; set; }
        public string ProductName { get; set; }
        public int QTY { get; set; }
        public decimal Price { get; set; }

    }
    public class OrderPayment
    {
        public int OrderPaymentId { get; set; }
        public Guid OrderId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal PaymentAmount { get; set; }
        public bool IsSuccessful { set; get; }
    }

    
}
