using System;
using System.Threading.Tasks;
using KibeberiUniversity.DataContext;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

/*28.09.2020 at 15:01hrs each request is handled within its own transaction neat!!*/
namespace KibeberiUniversity.Infrastructure
{
    public class DbContextTransactionPageFilter : IAsyncPageFilter
    {
        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)=>Task.CompletedTask;

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
         PageHandlerExecutionDelegate next)
         {
             var dbContext=context.HttpContext.RequestServices.GetService<UniversityDbContext>();

             try
             {
                 await dbContext.BeginTransactionAsync();

                 var actionExecuted=await next();

                 if(actionExecuted.Exception != null && !actionExecuted.ExceptionHandled)
                 {
                     dbContext.RollbackTransaction();
                 }
                 else
                 {
                     await dbContext.CommitTransactionAsync();
                 }
             }
             catch (System.Exception)
             {
                 dbContext.RollbackTransaction();
                 throw;
             }

         }
        
    }
}