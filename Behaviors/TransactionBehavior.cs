using System;
using System.Threading;
using KibeberiUniversity.DataContext;
using MediatR;
using System.Threading.Tasks;

namespace KibeberiUniversity.Behaviors
{
    public class TransactionBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest,TResponse>
    {
        private readonly UniversityDbContext _dbContext;

        public TransactionBehavior(UniversityDbContext context)=> _dbContext=context;

        public async Task<TResponse> Handle(TRequest request,
        CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                await _dbContext.BeginTransactionAsync();
                var response=await next();
                await _dbContext.CommitTransactionAsync();
                return response;
            }
            catch(Exception)
            {
              _dbContext.RollbackTransaction();
              throw;
            }
        }
    }
}