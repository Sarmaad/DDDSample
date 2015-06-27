using System;
using Domain.Infrastructure.Interfaces;

namespace Domain.Infrastructure
{
    public sealed class QueryResult<TResult>
    {
        public TResult[] Results { get; set; }
        public int TotalFound { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get { return PageSize != 0 ? (int)Math.Ceiling((decimal)TotalFound / PageSize) : 0; } }
    }

    public abstract class QueryBase<TResult> : IQuery<TResult>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

}