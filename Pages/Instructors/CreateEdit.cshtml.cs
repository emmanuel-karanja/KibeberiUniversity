using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models;
using MediatR;
using FluentValidation;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace KibeberiUniversity.Pages.Instructors
{
    public class CreateEditModel : PageModel
    {
        private readonly IMediator _mediator;

        public CreateEditModel(IMediator mediator)=> _mediator=mediator;

        [BindProperty]
        public CreateEditInstructorCommand Data {get;set;}

        public async Task OnGetCreateAsync()=> Data=await _mediator.Send(new Query());

        public async Task<IActionResult> OnPostCreateAsync()
        {
            await _mediator.Send(Data);
            
            return RedirectToPage("Index");
        }

        public async Task OnGetEditAsync(Query query)=>Data=await _mediator.Send(query);

        public async Task<IActionResult> OnPostEditAsync()
        {
            await _mediator.Send(Data);
            return RedirectToPage("Index");
        }

        public class Query : IRequest<CreateEditInstructorCommand>
        {
            public int? Id{get;set;}
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m=>m.Id).NotNull();
            }
        }

        public class CreateEditInstructorCommand : IRequest<int>
        {
            public CreateEditInstructorCommand()
            {
                AssignedCourses=new List<AssignedCourseData>();
                CourseAssignments=new List<CourseAssignment>();
                SelectedCourses=new string[0];
            }

            public int? Id {get;set;}

            public string LastName{ get;set;}

            [Display(Name="First Name")]
            public string FirstMidName{get;set;}

            [DisplayFormat(DataFormatString="{0:dd-DD-yyyy}")]
            public DateTime? HireDate{get;set;}

            [Display(Name="Location")]
            public string OfficeAssignmentLocation{get;set;}

            [IgnoreMap]
            public string[] SelectedCourses{get;set;}

            [IgnoreMap]
            public List<AssignedCourseData> AssignedCourses {get;set;}

            public List<CourseAssignment> CourseAssignments {get;set;}

            public class AssignedCourseData
            {
                public int CourseId {get;set;}
                public string Title{get;set;}
                public bool Assigned{get;set;}
            }
            public class CourseAssignment
            {
                public int CourseId{get; set;}
            }
        }

        public class CreateEditInstructorCommandValidator : AbstractValidator<CreateEditInstructorCommand>
        {
            public CreateEditInstructorCommandValidator()
            {
                 RuleFor(m=> m.LastName).NotNull().Length(3,50);
                 RuleFor(m=>m.FirstMidName).NotNull().Length(3,50);
                 RuleFor(m=> m.HireDate).NotNull();
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Instructor,CreateEditInstructorCommand>();
                CreateMap<CourseAssignment,CreateEditInstructorCommand.CourseAssignment>();
            }
        }

        public class QueryHandler : IRequestHandler<Query,CreateEditInstructorCommand>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context,
            IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

           public async Task<CreateEditInstructorCommand> Handle(Query query, CancellationToken ct)
           {
            CreateEditInstructorCommand model;
            if(query.Id == null)
            {
                model =new CreateEditInstructorCommand();
            }
            else
            {
                model = await _dbContext.Instructors
                                        .Where(i=>i.Id==query.Id)
                                        .ProjectTo<CreateEditInstructorCommand>(_config)
                                        .SingleOrDefaultAsync(ct);
            }

            PopulateAssignedCourseData(model);

            return model;
            }

            private void PopulateAssignedCourseData(CreateEditInstructorCommand model)
            {
                var allCourses=_dbContext.Courses;

                var instructorCourses=new HashSet<int>(model.CourseAssignments.Select(c=>c.CourseId));
                var viewModel=allCourses.Select(course=>new CreateEditInstructorCommand.AssignedCourseData
                {
                    CourseId=course.Id,
                    Title=course.Title,
                    Assigned=instructorCourses.Any() && instructorCourses.Contains(course.Id)
                }).ToList();

                model.AssignedCourses=viewModel;
            }
       }

       public class CreateEditInstructorCommandHandler : IRequestHandler<CreateEditInstructorCommand,int>
       {
           private readonly UniversityDbContext _dbContext;
           public CreateEditInstructorCommandHandler(UniversityDbContext context)
           {
               _dbContext=context;
           }

           public async Task<int> Handle(CreateEditInstructorCommand request, CancellationToken ct)
           {
               Instructor instructor;
               //creating no baai
               if(request.Id== null)
               {
                   instructor=new Instructor();
               }
               else  ///editing
               {
                    instructor = await _dbContext.Instructors     
                                         .Include(i => i.OfficeAssignment)
                                         .Include(i => i.CourseAssignments)
                                         .Where(i=> i.Id ==request.Id)
                                         .SingleAsync(ct);
               }

               var courses= await _dbContext.Courses.ToListAsync(ct);

               instructor.Handle(request,courses);

               await _dbContext.SaveChangesAsync(ct);

               return instructor.Id;
           }
       }
    }
}