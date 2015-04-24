using System;
namespace Messages.Events
{
    public interface IOrderCreated
    {
        Guid OrderId { get; set; }
    }
}
