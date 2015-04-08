using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure
{
    public class ConventionEventRouter : IRouteEvents
    {
        readonly bool throwOnApplyNotFound;
        readonly IDictionary<Type, Action<object>> handlers = new Dictionary<Type, Action<object>>();
        IAggregate registered;

        public ConventionEventRouter()
            : this(true)
        {
        }

        public ConventionEventRouter(bool throwOnApplyNotFound)
        {
            this.throwOnApplyNotFound = throwOnApplyNotFound;
        }

        public ConventionEventRouter(bool throwOnApplyNotFound, IAggregate aggregate)
            : this(throwOnApplyNotFound)
        {
            Register(aggregate);
        }

        public virtual void Register<T>(Action<T> handler)
        {
            if (handler == null)
                throw new ArgumentNullException("handler");

            this.Register(typeof (T), @event => handler((T) @event));
        }

        public virtual void Register(IAggregate aggregate)
        {
            if (aggregate == null)
                throw new ArgumentNullException("aggregate");

            this.registered = aggregate;

            // Get instance methods named Apply with one parameter returning void
            var applyMethods = aggregate.GetType()
                                        .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                        .Where(
                                               m =>
                                               m.Name == "When" && m.GetParameters().Length == 1 && m.ReturnParameter.ParameterType == typeof (void))
                                        .Select(m => new
                                                     {
                                                         Method = m,
                                                         MessageType = m.GetParameters().Single().ParameterType
                                                     });

            foreach (var apply in applyMethods)
            {
                var applyMethod = apply.Method;
                this.handlers.Add(apply.MessageType, m => applyMethod.Invoke(aggregate, new[] {m as object}));
            }
        }

        public virtual void Dispatch(object eventMessage)
        {
            if (eventMessage == null)
                throw new ArgumentNullException("eventMessage");

            Action<object> handler;
            if (this.handlers.TryGetValue(eventMessage.GetType(), out handler))
                handler(eventMessage);
            else if (this.throwOnApplyNotFound)
                this.registered.ThrowHandlerNotFound(eventMessage);
        }

        void Register(Type messageType, Action<object> handler)
        {
            this.handlers[messageType] = handler;
        }
    }

    internal static class AggregateExtensionMethods
    {
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format ?? string.Empty, args);
        }

        public static void ThrowHandlerNotFound(this IAggregate aggregate, object eventMessage)
        {
            var exceptionMessage = "Aggregate of type '{0}' raised an event of type '{1}' but not handler could be found to handle the message."
                .FormatWith(aggregate.GetType().Name, eventMessage.GetType().Name);

            throw new HandlerForDomainEventNotFoundException(exceptionMessage);
        }
    }

    public class HandlerForDomainEventNotFoundException : Exception
    {
        public HandlerForDomainEventNotFoundException()
        {
        }

        public HandlerForDomainEventNotFoundException(string message)
            : base(message)
        {
        }

        public HandlerForDomainEventNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public HandlerForDomainEventNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
