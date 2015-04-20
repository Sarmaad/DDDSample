using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Host.Infrastructure
{
    public static class NSBExtentions
    {
        public static ConventionsBuilder UseDDDSampleConventions(this ConventionsBuilder builder)
        {
            return builder
                .DefiningCommandsAs(t => t.Namespace != null &&  t.Namespace.StartsWith("Messages.Commands"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("Messages.Events"))
                .DefiningEncryptedPropertiesAs(p => p.Name.StartsWith("Encrypted"))
                .DefiningDataBusPropertiesAs(p => p.Name.EndsWith("DataBus"))
                .DefiningExpressMessagesAs(t => t.Name.EndsWith("Express"))

                ;
        }

    }
}
