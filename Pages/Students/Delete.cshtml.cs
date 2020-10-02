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
using Microsoft.AspNetCore.Mvc;

namespace KibeberiUniversity.Pages.Students
{
    public class DeleteModel : PageModel
    {
        private readonly IMediator _mediator;
        public DeleteModel(IMediator mediator)=>_mediator=mediator;

        [BindProperty]
        public DeleteStudentCommand Data{get;set;}

        public async Task OnGetAsync(Query query)=>Data=await _mediator.Send(query);

        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);

            return RedirectToPage("Index");
        }

        public class Query : IRequest<DeleteStudentCommand>
        {
            public int Id{get;set;}
        }

        public class DeleteStudentCommand :IRequest
        {
            public int Id{get;set;}

            [Display(Name="First Name")]
            public string FirstMidName{get;set;}

            public string LastName{get;set;}

            public DateTime EnrollmentDate{get;set;}
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()=> CreateMap<Student,DeleteStudentCommand>();
        }

        public class QueryHandler : IRequestHandler<Query, DeleteStudentCommand>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly  IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context, IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<DeleteStudentCommand> Handle(Query query, CancellationToken ct)
            {
                return await _dbContext.Students.Where(s=>s.Id==query.Id)
                                                .ProjectTo<DeleteStudentCommand>(_config)
                                                .SingleOrDefaultAsync(ct);
            }
        }

        public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand,Unit>
        {
            private readonly UniversityDbContext _dbContext;

            public DeleteStudentCommandHandler(UniversityDbContext context)
            {
                _dbContext=context;
            }

            public async Task<Unit> Handle(DeleteStudentCommand request, CancellationToken ct)
            {
                _dbContext.Students.Remove(await _dbContext.Students.FindAsync(request.Id));
                await _dbContext.SaveChangesAsync(ct);
                return default;
            }
        }
    }
}