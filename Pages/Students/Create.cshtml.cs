using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
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
using FluentValidation;
using AutoMapper.Configuration;

namespace KibeberiUniversity.Pages.Students
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;
        public CreateModel(IMediator mediator)=> _mediator=mediator;

        [BindProperty]
        public CreateStudentCommand Data{get;set;}

        public void OnGet()=> Data= new CreateStudentCommand();

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return RedirectToPage("Index");
        }

        public class CreateStudentCommand : IRequest<int>
        {
             public string LastName{get;set;}
             
             [Display(Name="First Name")]
             public string FirstMidName {get;set;}

             public DateTime? EnrollmentDate{get;set;}
        }

        public class MappingProfile :Profile
        {
            public MappingProfile()=>CreateMap<CreateStudentCommand,Student>(MemberList.Source);
        }

        public class Validator :AbstractValidator<CreateStudentCommand>
        {
            public Validator()
            {
                RuleFor(m=>m.LastName).NotNull().Length(2,50);
                RuleFor(m=>m.FirstMidName).NotNull().Length(2,50);
                RuleFor(m=>m.EnrollmentDate).NotNull();
            }
        }

        public class Handler : IRequestHandler<CreateStudentCommand,int>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(UniversityDbContext context, IMapper mapper)
            {
                _dbContext=context;
                _mapper=mapper;
            }

            public async Task<int> Handle(CreateStudentCommand request, CancellationToken ct)
            {
                var student=_mapper.Map<CreateStudentCommand,Student>(request);
                _dbContext.Add(student);
                await _dbContext.SaveChangesAsync(ct);

                return student.Id;
            }
        }
    }
}