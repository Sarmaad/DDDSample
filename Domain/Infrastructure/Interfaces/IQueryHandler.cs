namespace Domain.Infrastructure.Interfaces
{
    public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        QueryResult<TResult> Handle(TQuery query);
    }
}