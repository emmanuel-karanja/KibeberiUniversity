using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models;
using MediatR;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace KibeberiUniversity.Pages.Instructors
{
    public class DeleteModel : PageModel
    {
        private readonly IMediator _mediator;
        public DeleteModel(IMediator mediator)=> _mediator=mediator;

        [BindProperty]
        public DeleteInstructorCommand Data{get;set;}

        public async Task OnGetAsync(Query query)=> Data=await _mediator.Send(query);

        public async Task<ActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return  RedirectToPage("Index");
        }

        public class Query : IRequest<DeleteInstructorCommand>
        {
            public int? Id{get;set;}
        }

        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m=>m.Id).NotNull();
            }
        }
        
        public class DeleteInstructorCommand : IRequest
        {
            public int? Id{get;set;}
            public string LastName{get;set;}
            [Display(Name="First Name")]
            public string FirstMidName{get;set;}

            [DisplayFormat(DataFormatString="{0:dd-MM-yyyy}")]
            public DateTime? HireDate {get;set;}

            [Display(Name="Location")]
            public string OfficeAssignmentLocation {get;set;}
        }
        public class MappingProfile : Profile
        {
            public MappingProfile()=>CreateMap<Instructor, DeleteInstructorCommand>();
        }

        public class QueryHandler : IRequestHandler<Query,DeleteInstructorCommand>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context, IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<DeleteInstructorCommand> Handle(Query query, CancellationToken ct)
            {
                return  await _dbContext.Instructors.Where(x=>x.Id==query.Id)
                                                    .ProjectTo<DeleteInstructorCommand>(_config)
                                                    .SingleOrDefaultAsync(ct);
            }
        }

        public class DeleteInstructorCommandHandler : IRequestHandler<DeleteInstructorCommand,Unit>
        {
           private readonly UniversityDbContext _dbContext;

           public DeleteInstructorCommandHandler(UniversityDbContext context)
           {
               _dbContext=context;
           }

           public async Task<Unit> Handle(DeleteInstructorCommand request, CancellationToken ct)
           {
               Instructor instructor=await _dbContext.Instructors.Include(i => i.OfficeAssignment)
                                                                .Where(i=>i.Id==request.Id)
                                                                .SingleAsync(ct);
               instructor.Handle(request);

               _dbContext.Instructors.Remove(instructor);

               var department = await _dbContext.Departments.Where(d=>d.InstructorId==request.Id)
                                                            .SingleOrDefaultAsync(ct);

               if(department != null)
               {
                   department.InstructorId=null;
               }

               return default;
           }
        }
        
    }
}