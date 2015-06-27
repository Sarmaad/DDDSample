
using System.Configuration;
using Domain;
using Domain.Infrastructure;
using Domain.Infrastructure.Interfaces;
using Domain.Storage.EF;
using Host.Common;
using Host.Infrastructure;
using Ninject;
using NServiceBus.Logging;
using NServiceBus.UnitOfWork;

namespace Host
{
    using NServiceBus;

    /*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://particular.net/articles/the-nservicebus-host
	*/
    [EndpointName("DDDSample.Host")]
    public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        public void Customize(BusConfiguration configuration)
        {
            configuration.RegisterComponents(components => components.ConfigureComponent<IRepository>(DependencyLifecycle.InstancePerUnitOfWork));
            configuration.RegisterComponents(components => components.ConfigureComponent<IManageUnitsOfWork>(DependencyLifecycle.InstancePerUnitOfWork));
            configuration.UseContainer<NinjectBuilder>(k => k.ExistingKernel(CreateKernel()));
            configuration.UsePersistence<NHibernatePersistence>();
            configuration.UseTransport<MsmqTransport>();
            configuration.RijndaelEncryptionService();
            configuration.Transactions().DisableDistributedTransactions(); // we need to use NSB without MSDTC
            configuration.Conventions().UseDDDSampleConventions();
        }

        IKernel CreateKernel()
        {
            var kernal = new StandardKernel();

            kernal.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.DeclaringType));
            kernal.Bind<IAppContext>().To<AppContext>().WithConstructorArgument("connectionString", ConfigurationManager.ConnectionStrings["Database"]);
            kernal.Bind<IManageUnitsOfWork>().To<UnitOfWork>();

            return kernal;
        }
    }
}
