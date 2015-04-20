using System;
using Domain.Infrastructure;
using NServiceBus.UnitOfWork;

namespace Host.Infrastructure
{
    public class UnitOfWork : IManageUnitsOfWork
    {
        readonly IRepository _repository;

        public UnitOfWork(IRepository repository)
        {
            _repository = repository;
        }

        public void Begin()
        {
            
        }

        public void End(Exception ex = null)
        {
            if (ex != null)
                return;

            ((IDisposable)_repository).Dispose();
        }
    }
}
