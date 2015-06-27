using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure
{
    public interface IRouteEvents
    {
        void Register<T>(Action<T> handler);
        void Register(IAggregate aggregate);

        void Dispatch(object eventMessage);
    }
}
