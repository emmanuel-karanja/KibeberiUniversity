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


namespace KibeberiUniversity.Pages.Courses
{
    public class DeleteModel : PageModel
    {
        private readonly IMediator _mediator;

        public DeleteModel(IMediator mediator)
        {
            _mediator=mediator;
        }
        
        [BindProperty]
        public DeleteCourseCommand Data{get;set;}

        public async Task OnGetAsync(Query query)
        {
            Data=await _mediator.Send(query);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);
             return RedirectToPage("Index");
        }
        
       public class Query: IRequest<DeleteCourseCommand>
       {
           public int? Id {get;set;}
           
       }

       public class QueryValidator : AbstractValidator<Query>
       {
           public QueryValidator()
           {
               RuleFor(m=>m.Id).NotNull();
           }
       }

       public class MappingProfile : Profile
       {
           public MappingProfile()=>CreateMap<Course,DeleteCourseCommand>();
       }

       public class QueryHandler : IRequestHandler<Query, DeleteCourseCommand>
       {
           private readonly UniversityDbContext _dbContext;
           private readonly IConfigurationProvider _config;

           public QueryHandler(UniversityDbContext context, IConfigurationProvider config)
           {
               _dbContext=context;
               _config=config;
           }

           public async Task<DeleteCourseCommand> Handle(Query query, CancellationToken ct)
           {
               return await _dbContext.Courses.Where(x=>x.Id==query.Id)
                                              .ProjectTo<DeleteCourseCommand>(_config)
                                              .SingleOrDefaultAsync(ct);
           }

           
       }
       public class DeleteCourseCommand : IRequest
        {
               [Display(Name="Number")]
               public int Id {get;set;}
               public string Title{get;set;}

               public int Credits {get;set;}

               [Display(Name="Department")]
               public string DepartmentName {get;set;}
        }

           public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand,Unit>
           {
               private readonly UniversityDbContext _dbContext;
               public DeleteCourseCommandHandler(UniversityDbContext context)
               {
                   _dbContext=context;
               }

               public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken ct)
               {
                   var course=await _dbContext.Courses.FindAsync(request.Id);
                
                   _dbContext.Remove(course);
                   await _dbContext.SaveChangesAsync();

                   return default;
               }
          }
    }
}