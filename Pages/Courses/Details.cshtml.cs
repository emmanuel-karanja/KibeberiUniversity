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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KibeberiUniversity.Pages.Courses
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DetailsModel> _logger;

        public DetailsModel(IMediator mediator, ILogger<DetailsModel> logger)
        {
            _mediator=mediator;
            _logger=logger;
        }

        public CourseModel Data {get;private set;}

        public async Task OnGetAsync(Query query)
        {
            Data=await _mediator.Send(query);
        }

        public class Query : IRequest<CourseModel>
        {
            public int? Id {get;set;}
        }

        public class QueryValidator :AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m=>m.Id).NotNull();
            }
        }

        public class CourseModel
        {
            public int Id {get;set;}
            public string Title{get;set;}
            public int Credits{get;set;}

            [Display(Name="Department")]
            public string DepartmentName{get;set;}
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()=> CreateMap<Course,CourseModel>();
        }

        public class QueryHandler : IRequestHandler<Query,CourseModel>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IMapper _mapper;
            private readonly ILogger<QueryHandler> _logger;

            public QueryHandler(UniversityDbContext context, IMapper mapper, ILogger<QueryHandler> logger)
            {
                _dbContext=context;
                _mapper=mapper;
                _logger=logger;
            }

            public async  Task<CourseModel> Handle(Query query,CancellationToken ct)
            {
                var course= await _dbContext.Courses.Where(x=>x.Id==query.Id)
                                           .SingleOrDefaultAsync(ct);
               _logger.LogInformation($"loaded course departmentId{0}",course.DepartmentId);
               return _mapper.Map<Course,CourseModel>(course);
            }
        }       
    }
}