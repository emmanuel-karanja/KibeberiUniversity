using Dapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace KibeberiUniversity.DataContext
{
    public class DapperDbQueryFacade : IDbQueryFacade
    {
        private readonly UniversityDbContext _context;

        public DapperDbQueryFacade(UniversityDbContext context)=> _context=context;

        public async Task<int> ExecuteAsync(string sql, object param=null, CancellationToken ct=default)
        {
           return await _context.Database.GetDbConnection().ExecuteAsync(sql,param);
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param=null, CancellationToken ct=default)
        {
            return (await _context.Database.GetDbConnection().QueryAsync<T>(sql,param)).AsList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param=null, CancellationToken ct=default)
        {
            return await _context.Database.GetDbConnection().QueryFirstOrDefaultAsync<T>(sql,param);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object param=null, CancellationToken ct=default)
        {
            return await _context.Database.GetDbConnection().QuerySingleAsync<T>(sql,param);
        }
    }
}