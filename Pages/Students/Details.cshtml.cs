using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace KibeberiUniversity.Pages.Students
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;

        public DetailsModel(IMediator mediator)=> _mediator=mediator;

        public Model Data{get;private set;}

        public async Task OnGetAsync(Query query)=>Data=await _mediator.Send(query);

        public class Query :IRequest<Model>
        {
            public int Id{get;set;}
        }

        public class Model
        {
            public int Id{get;set;}

            [Display(Name="First Name")]
            public string FirstMidName {get;set;}

            public string LastName{get;set;}

            public DateTime EnrollmentDate{get;set;}

            public List<Enrollment> Enrollments {get;set;}

            public class Enrollment
            {
                public string CourseTitle{get;set;}
                public Grade? Grade{get;set;}
            }
        }

        public class MappingProfile :Profile
        {
            public MappingProfile()
            {
                CreateMap<Student,Model>();
                CreateMap<Enrollment,Model.Enrollment>();
            }
        }

        public class QueryHandler: IRequestHandler<Query,Model>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context, IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<Model> Handle(Query query, CancellationToken ct)
            {
                return await _dbContext.Students.Where(s=>s.Id==query.Id)
                                                .ProjectTo<Model>(_config)
                                                .SingleOrDefaultAsync(ct);
            }
        }
    }
}