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
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;

        public DetailsModel(IMediator mediator)
        {
            _mediator=mediator;
        }
        
        public DepartmentModel Data {get;private set;}

        public async Task OnGetAsync(Query query)
        {
            Data=await _mediator.Send(query);
        }

        public class Query : IRequest<DepartmentModel>
        {
            public int Id {get;set;}
        }

        public class DepartmentModel
        {
            public string Name {get;set;}
            public decimal Budget{get;set;}
            public DateTime StartDate{get;set;}
            public int Id {get;set;}

            [Display(Name="Administrator")]
            public string AdministratorFullName {get;set;}
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()=> CreateMap<Department,DepartmentModel>();
        }

        public class QueryHandler : IRequestHandler<Query,DepartmentModel>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context,
            IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<DepartmentModel> Handle(Query query, CancellationToken ct)
            {
                return await _dbContext.Departments.Where(x=>x.Id==query.Id)
                                            .ProjectTo<DepartmentModel>(_config)
                                            .SingleOrDefaultAsync(ct);
            }
        }
    }
}