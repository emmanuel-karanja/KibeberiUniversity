using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KibeberiUniversity.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator=mediator;
        }

        public Result Data {get;private set;}

        public async Task OnGetAsync()=> Data =await _mediator.Send(new Query());

        public class Query: IRequest<Result>
        {

        }

        public class Result
        {
            public List<Course> Courses {get;set;}
            public class Course
            {
                public int Id{get;set;}
                public string Title{get;set;}
                public int Credits{get;set;}
                public string DepartmentName{get;set;}
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()=> CreateMap<Course,Result.Course>();
        }

        public class Handler: IRequestHandler<Query,Result>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public Handler(UniversityDbContext context, IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public  async Task<Result> Handle(Query query, CancellationToken ct)
            {
                var courses=await _dbContext.Courses
                                            .OrderBy(d=>d.Id)
                                            .ProjectToListAsync<Result.Course>(_config);
                return new Result
                {
                    Courses=courses
                };
            }
        }        
    }
}