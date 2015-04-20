using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;

namespace Domain.Infrastructure
{
    public interface ISpecificationFactory
    {
        TSpecification Get<TSpecification>();
    }

    public class SpecificationFactory:ISpecificationFactory
    {
        readonly IKernel _kernel;

        public SpecificationFactory(IKernel kernel)
        {
            _kernel = kernel;
        }

        public TSpecification Get<TSpecification>()
        {
            return _kernel.Get<TSpecification>();
        }
    }
}
