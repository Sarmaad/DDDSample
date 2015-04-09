using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Infrastructure
{
    public interface IQuery<TResult>
    {
        QueryResult<TResult> Execute(IAppContext context);
    }

    public sealed class QueryResult<TResult>
    {
        public TResult[] Results { get; set; }
        public int TotalFound { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
