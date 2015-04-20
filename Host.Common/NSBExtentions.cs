using NServiceBus;

namespace Host.Common
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
