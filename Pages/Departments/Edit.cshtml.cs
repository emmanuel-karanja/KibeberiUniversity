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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace KibeberiUniversity.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly IMediator _mediator;
        public EditModel(IMediator mediator)=>_mediator=mediator;

        [BindProperty]
        public EditDepartmentCommand Data{get;set;}

        public async Task OnGetAsync(Query query)
        {
            Data=await _mediator.Send(query);
        }

        public async Task<ActionResult> OnPostAsync(int id)
        {
            await _mediator.Send(Data);

            return RedirectToPage("Index");
        }

        public class Query : IRequest<EditDepartmentCommand>
        {
            public int Id {get;set;}
        }

        public class EditDepartmentCommand : IRequest
        {
            public string Name {get;set;}
            public decimal? Budget {get;set;}

            public DateTime? StartDate{get;set;}

            public Instructor Administrator {get;set;}
            public int Id {get;set;}

            public byte[] RowVersion {get;set;}
        }

        public class Validator : AbstractValidator<EditDepartmentCommand>
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
            public MappingProfile()=> CreateMap<Department,EditDepartmentCommand>();
        }

        public class QueryHandler : IRequestHandler<Query, EditDepartmentCommand>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context,
             IConfigurationProvider config)
             {
                 _dbContext=context;
                 _config=config;
             }

             public async Task<EditDepartmentCommand> Handle(Query query, CancellationToken ct)
             {
                 return await _dbContext.Departments.Where(x=>x.Id==query.Id)
                                                   .ProjectTo<EditDepartmentCommand>(_config)
                                                   .SingleOrDefaultAsync(ct);
             }

         public class EditDepartmentCommandHandler : IRequestHandler<EditDepartmentCommand,Unit>
         {
             private readonly UniversityDbContext _dbContext;
             private readonly IMapper _mapper;
             public EditDepartmentCommandHandler(UniversityDbContext context,
             IMapper mapper)
             {
                 _dbContext=context;
                 _mapper=mapper;
             }

             public async Task<Unit> Handle(EditDepartmentCommand request,CancellationToken ct)
             {
                 var department=await _dbContext.Departments.FindAsync(request.Id);

                 request.Administrator=await _dbContext.Instructors.FindAsync(request.Administrator.Id);

                 _mapper.Map(request, department);
                 return default;
             }

         }
        }
    }
}