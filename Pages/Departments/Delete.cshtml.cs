using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace KibeberiUniversity.Pages.Departments
{
    public class DeleteModel: PageModel
    {
        private readonly IMediator _mediator;

        public DeleteModel(IMediator mediator)
        {
            _mediator=mediator;
        }

        //the date model
         [BindProperty]
          public DeleteDepartmentCommand Data {get;set;}

          public async Task OnGetAsync(Query query)
          {
              Data=await _mediator.Send(query);
          }

          public async Task<ActionResult> OnPostAsync()
          {
              await _mediator.Send(Data);

              return RedirectToPage("Index");
          }

          public class Query : IRequest<DeleteDepartmentCommand>
          {
              public int Id {get;set;}
          }

          public class DeleteDepartmentCommand :IRequest
          {
              public string Name{get;set;}
              public decimal Budget{get;set;}

              public DateTime StartDate{get;set;}

              public int Id{get;set;}

              [Display(Name="Administrator")]
              public string AdministratorFullName{get;set;}
              public byte[] RowVersion {get;set;}
          }

          public class MappingProfile : Profile
          {
              public MappingProfile()=> CreateMap<Department,DeleteDepartmentCommand>();
          }

          public class QueryHandler : IRequestHandler<Query,DeleteDepartmentCommand>
          {
              private readonly UniversityDbContext _dbContext;
              private readonly IConfigurationProvider _config;

              public QueryHandler(UniversityDbContext context,
              IConfigurationProvider config)
              {
                  _dbContext=context;
                  _config=config;
              }

              public async Task<DeleteDepartmentCommand> Handle(Query query, CancellationToken ct)
              {
                  return await _dbContext.Departments                
                                         .Where(x=>x.Id==query.Id)
                                         .ProjectTo<DeleteDepartmentCommand>(_config)
                                         .SingleOrDefaultAsync();
              }

              public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand,Unit>
              {
                  private readonly UniversityDbContext _dbContext;
                  public DeleteDepartmentCommandHandler(UniversityDbContext context)
                  {
                      _dbContext=context;
                  }

                  public async Task<Unit> Handle(DeleteDepartmentCommand request,CancellationToken ct)
                  {
                      var department=await _dbContext.Departments.FindAsync(request.Id);

                      _dbContext.Departments.Remove(department);
                      await _dbContext.SaveChangesAsync(ct);
                      return default;
                  }
              }
          }
        
    }
}