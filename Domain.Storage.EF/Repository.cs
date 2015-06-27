using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Domain.Infrastructure;
using Domain.Infrastructure.Interfaces;

namespace Domain.Storage.EF
{
    public sealed class Repository : IRepository, IDisposable
    {
        readonly IAppContext _context;

        public Repository(IAppContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add a new entity to database
        /// </summary>
        public void Add<TAggregate>(TAggregate aggregate) where TAggregate : class
        {
            ((AppContext) _context).Set<TAggregate>().Add(aggregate);
        }

        /// <summary>
        /// Load an entity by Id with optional includes
        /// </summary>
        public TAggregate Load<TAggregate>(Func<TAggregate, bool> predicate, params Expression<Func<TAggregate, object>>[] includes)
            where TAggregate : class
        {
            var q = ((AppContext) _context).Set<TAggregate>().AsQueryable();

            if (includes != null)
            {
                q = includes.Aggregate(q, (set, inc) => set.Include(inc));
            }

            return q.SingleOrDefault(predicate);
        }

        public QueryResult<TResult> Search<TResult>(IQuery<TResult> query)
        {
            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            var handler = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .SingleOrDefault(p => handlerType.IsAssignableFrom(p));

            if (handler == null) return null;

            dynamic q = Activator.CreateInstance(handler, _context);
            return q.Handle((dynamic)query);
        }
        public TProjection Project<TAggregate, TProjection>(Func<IQueryable<TAggregate>, TProjection> query) where TAggregate : class
        {
            return query(((AppContext)_context).Set<TAggregate>().AsQueryable());
        }
        public void Dispose()
        {
            _context.SaveChanges();
        }
    }
}
