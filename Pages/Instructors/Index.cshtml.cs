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

namespace KibeberiUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator)=> _mediator=mediator;

        public Model Data{get;private set;}

        public async Task OnGetAsync(Query query)=>Data=await _mediator.Send(query);

        public class Query : IRequest<Model>
        {
            public int? Id {get;set;}
            public int? CourseId {get;set;}
        }

        public class Model
        {
            public int? InstructorId{get;set;}
            public int? CourseId{get;set;}

            public IList<Instructor> Instructors{get;set;}
            public IList<Course> Courses {get;set;}
            public IList<Enrollment> Enrollments {get;set;}

            public class Instructor
            {
                public int Id{get;set;}
                [Display(Name="Last Name")]
                public string LastName{get;set;}

                [Display(Name="First Name")]
                public string FirstMidName{get;set;}

                [DisplayFormat(DataFormatString="{0:dd-MM-yyyy}",ApplyFormatInEditMode=true)]
                [Display(Name="Hire Date")]
                public DateTime HireDate {get;set;}

                public string OfficeAssignmentLocation{get;set;}

                public IEnumerable<CourseAssignment> CourseAssignments{get;set;}
            }

            public class CourseAssignment
            {
                public int CourseId{get;set;}
                public string CourseTitle{get;set;}
            }

            public class Course
            {
                public int Id {get;set;}
                public string Title{get;set;}
                public string DepartmentName{get;set;}
            }

            public class Enrollment
            {
                [DisplayFormat(NullDisplayText="No Grade")]
                public Grade? Grade {get;set;}

                public string StudentFullName {get;set;}
            }
        }
        
        public class MappingProfile :Profile
        {
            public MappingProfile()
            {
                CreateMap<Instructor, Model.Instructor>();
                CreateMap<CourseAssignment,Model.CourseAssignment>();
                CreateMap<Course,Model.Course>();
                CreateMap<Enrollment,Model.Enrollment>();
            }
        }

        public class QueryHandler : IRequestHandler<Query,Model>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context,
            IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<Model> Handle(Query query, CancellationToken ct)
            {
                //and now the big megilla of queries..
                //we fetch everything to memory and then do the join?
                /// <summary>
                ///28.09.2020 at 23:45hrs this needs to be looked into
                /// </summary>
                /// <returns></returns>

                var instructors = await _dbContext.Instructors
                          .Include(i=> i.CourseAssignments)
                          .ThenInclude(c=> c.Course)
                          .OrderBy(i=>i.LastName)
                          .ProjectTo<Model.Instructor>(_config)
                          .ToListAsync(ct);

                var courses=new List<Model.Course>();
                var enrollments= new List<Model.Enrollment>();   

                if(query.Id != null)
                {
                    courses = await _dbContext.CourseAssignments
                        .Where(ci=>ci.InstructorId == query.Id)
                        .Select(ci=>ci.Course)
                        .ProjectTo<Model.Course>(_config)
                        .ToListAsync(ct);
                } 

                if(query.CourseId != null)   
                {
                    enrollments=await _dbContext.Enrollments
                       .Where(x=>x.CourseId == query.CourseId)
                       .ProjectTo<Model.Enrollment>(_config)
                       .ToListAsync(ct);
                }          

                var viewModel= new Model
                {
                    Instructors=instructors,
                    Courses=courses,
                    Enrollments=enrollments,
                    InstructorId=query.Id,
                    CourseId=query.CourseId
                };

                return viewModel;   
            }
        }
    }
}