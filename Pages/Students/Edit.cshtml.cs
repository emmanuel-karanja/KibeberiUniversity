using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
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
        public class EditModel : PageModel
    {
        private readonly IMediator _mediator;
        public EditModel(IMediator mediator)=> _mediator=mediator;

        [BindProperty]
         public EditStudentCommand Data{get;set;}

        public async Task OnGetAsync(Query query)=> Data=await _mediator.Send(query);

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return RedirectToPage("Index");
        }

        public class Query : IRequest<EditStudentCommand>
        {
            public int Id{get;set;}
        }

        public class QueryValidator :AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m=>m.Id).NotNull();
            }
        }

        public class EditStudentCommand : IRequest
        {
            public int Id{get;set;}
            public string LastName{get;set;}

            [Display(Name="First Name")]
            public string FirstMidName{get;set;}

            public DateTime? EnrollmentDate{get;set;}
        }

        public class EditStudentCommandValidator : AbstractValidator<EditStudentCommand>
        {
            public EditStudentCommandValidator()
            {
                RuleFor(m=> m.LastName).NotNull().Length(2,50);
                RuleFor(m=>m.FirstMidName).NotNull().Length(2,50);
                RuleFor(m=>m.EnrollmentDate).NotNull();
            }
        }
        public class MappingProfile : Profile
        {
            public MappingProfile()=> CreateMap<Student,EditStudentCommand>().ReverseMap();
        }

        public class QueryHandler : IRequestHandler<Query,EditStudentCommand>
        {
            private readonly UniversityDbContext  _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context, IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<EditStudentCommand> Handle(Query query, CancellationToken ct)
            {
                return await _dbContext.Students.Where(s=>s.Id==query.Id)
                                                .ProjectTo<EditStudentCommand>(_config)
                                                .SingleOrDefaultAsync(ct);
            }
        }

        public class EditStudentCommandHandler : IRequestHandler<EditStudentCommand,Unit>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IMapper _mapper;

            public EditStudentCommandHandler(UniversityDbContext context, IMapper mapper)
            {
                _dbContext=context;
                _mapper=mapper;
            }

            public async Task<Unit> Handle(EditStudentCommand request, CancellationToken ct)
            {
                _mapper.Map(request, await _dbContext.Students.FindAsync(request.Id));

                return default;
            }
        }


    }
}