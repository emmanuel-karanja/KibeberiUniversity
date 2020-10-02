using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace KibeberiUniversity.DataContext
{
    public interface IDbQueryFacade
    {
         Task<int> ExecuteAsync(string sql, object param=null, CancellationToken cancellationToke=default);
         Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param=null,CancellationToken cancellationToken=default);
         Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param=null, CancellationToken cancellationToken=default);
         Task<T> QuerySingleAsync<T> (string sql, object param=null, CancellationToken cancellationToken=default);

    }
}