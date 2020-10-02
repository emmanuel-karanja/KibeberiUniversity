using  System.ComponentModel.DataAnnotations;
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
using Microsoft.Extensions.Logging;

namespace KibeberiUniversity.Pages.Courses
{
    public class EditModel : PageModel
    {
        private readonly IMediator _mediator;

        [BindProperty]
        public EditCourseCommand Data {get;set;}

        public EditModel(IMediator mediator)
        {
            _mediator=mediator;
        }

        public async Task OnGetAsync(Query query)=>Data= await _mediator.Send(query);
        
        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return RedirectToPage("Index");
        }

        public class Query :IRequest<EditCourseCommand>
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

        public class QueryHandler : IRequestHandler<Query,EditCourseCommand>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context,
            IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<EditCourseCommand> Handle(Query query,CancellationToken ct)
            {
               return await _dbContext.Courses    
                                      .Where(c=>c.Id==query.Id)
                                      .ProjectTo<EditCourseCommand>(_config)
                                      .SingleOrDefaultAsync(ct);
            }
        }

         public class EditCourseCommand :IRequest
          {
              [Display(Name="Number")]
              public int Id {get;set;}
              public string Title{get;set;}
              public int? Credits{get;set;}

              public Department Department{get;set;}
          }

          public class MappingProfile : Profile
          {
             public MappingProfile()=> CreateMap<Course,EditCourseCommand>().ReverseMap();
          }

          public class EditCourseCommandValidator : AbstractValidator<EditCourseCommand>
          {
              public EditCourseCommandValidator()
              {
                  RuleFor(m=>m.Title).NotNull().Length(3,50);
                  RuleFor(m=>m.Credits).NotNull().InclusiveBetween(0,5);
              }
          }

          public class EditCourseCommandHandler: IRequestHandler<EditCourseCommand,Unit>
          {
              private readonly UniversityDbContext _dbContext;
              private readonly IMapper _mapper;
              private readonly ILogger<EditCourseCommandHandler> _logger;

              public EditCourseCommandHandler(UniversityDbContext context, IMapper mapper,
              ILogger<EditCourseCommandHandler> logger)
              {
                  _dbContext=context;
                  _mapper=mapper;
                  _logger=logger;
              }

              public async Task<Unit> Handle(EditCourseCommand request,CancellationToken ct)
              {   
                  var course=await _dbContext.Courses.FindAsync(request.Id);
                  _mapper.Map(request,course);
                  //_logger.LogInformation($"the edited course's credits: {course.Credits}");
                  
                  return default;
              }
          }
    }
}