namespace Domain.Events.Orders
{
    public class OrderLineAdded
    {
        public string ProductName { get; set; }
        public int QTY { get; set; }
        public decimal Price { get; set; }
    }
}