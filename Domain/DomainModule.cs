using Domain.Infrastructure;
using Domain.Specifications;
using Ninject.Modules;

namespace Domain
{
    public class DomainModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository>().To<Repository>().InThreadScope();
            Bind<IDuplicateCustomerEmail>().To<DuplicateCustomerEmail>();
        }
    }
}
