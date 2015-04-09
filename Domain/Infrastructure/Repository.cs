using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Infrastructure
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


        void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Commit();
        }

        public QueryResult<TResult> Search<TResult>(IQuery<TResult> query)
        {
            return query.Execute(_context);
        }

        public TProjection Project<TAggregate, TProjection>(Func<IQueryable<TAggregate>, TProjection> query) where TAggregate : class
        {
            return query(((AppContext)_context).Set<TAggregate>().AsQueryable());
        }
    }
}
