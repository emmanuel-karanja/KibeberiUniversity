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
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace KibeberiUniversity.Pages.Departments
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;

        public CreateModel(IMediator mediator)
        {
            _mediator=mediator;
        }

        [BindProperty]
        public CreateDepartmentCommand Data {get;set;}

        public async Task<ActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return RedirectToPage("Index");
        }

        public class Validator : AbstractValidator<CreateDepartmentCommand>
        {
            public Validator()
            {
                RuleFor(m=>m.Name).NotNull().Length(3,50);
                RuleFor(m=>m.Budget).NotNull();
                RuleFor(m=>m.StartDate).NotNull();
                RuleFor(m=>m.Administrator).NotNull();
            }
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()=>CreateMap<CreateDepartmentCommand,Department>(MemberList.Source);
        }

        public class CreateDepartmentCommand : IRequest<int>
        {
            [StringLength(50,MinimumLength=3)]
            public string Name {get;set;}

            [DataType(DataType.Currency)]
            [Column(TypeName="money")]
            public decimal? Budget {get;set;}
            
            [DataType(DataType.Date)]
            [DisplayFormat(DataFormatString="{0:dd-MM-yyyy}",ApplyFormatInEditMode=true)]
            public DateTime? StartDate {get;set;}

            public Instructor Administrator{get;set;}
        }

        public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand,int>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IMapper _mapper;

            public CreateDepartmentCommandHandler(
                UniversityDbContext context,
                IMapper mapper
            )
            {
                _dbContext=context;
                _mapper=mapper;
            }

            public async Task<int> Handle(CreateDepartmentCommand request, CancellationToken ct)
            {
                var department=_mapper.Map<CreateDepartmentCommand,Department>(request);
                _dbContext.Departments.Add(department);

                await _dbContext.SaveChangesAsync(ct);

                return department.Id;
            }

        }
    }
}