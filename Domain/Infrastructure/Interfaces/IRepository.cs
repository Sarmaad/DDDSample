using System;
using System.Linq;
using System.Linq.Expressions;

namespace Domain.Infrastructure.Interfaces
{
    public interface IRepository
    {
        void Add<TAggregate>(TAggregate aggregate) where TAggregate : class;
        TAggregate Load<TAggregate>(Func<TAggregate, bool> predicate, params Expression<Func<TAggregate, object>>[] includes) where TAggregate : class;
        QueryResult<TResult> Search<TResult>(IQuery<TResult> query);
        TProjection Project<TAggregate, TProjection>(Func<IQueryable<TAggregate>, TProjection> query) where TAggregate : class;
    }
}