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
using System.ComponentModel.DataAnnotations;
using AutoMapper.Configuration;
using System.Linq;

namespace KibeberiUniversity.Pages.Instructors
{
    public class DetailsModel : PageModel
    {
        private readonly IMediator _mediator;

        public DetailsModel(IMediator mediator)=>_mediator=mediator;

        public InstructorModel Data{get;private set;}

        public async Task OnGetAsync(Query query)=> Data=await _mediator.Send(query);

        public class Query : IRequest<InstructorModel>
        {
            public int? Id {get;set;}
        }

        public class Validator :AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m=>m.Id).NotNull();
            }
        }

        public class InstructorModel
        {
            public int? Id {get;set;}

            public string LastName{get;set;}

            [Display(Name="First Name")]
            public string FirstMidName {get;set;}

            [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
            public DateTime? HireDate{get;set;}

            [Display(Name="Location")]
            public string OfficeAssignmentLocation {get;set;}

        }

        public class MappingProfile : Profile
        {
            public MappingProfile()=>CreateMap<Instructor,InstructorModel>();
        }

        public class QueryHandler : IRequestHandler<Query, InstructorModel>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context,
            IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<InstructorModel> Handle(Query query, CancellationToken ct)
            {
                return await _dbContext.Instructors.Where(i=> i.Id==query.Id)
                                                   .ProjectTo<InstructorModel>(_config)
                                                   .SingleOrDefaultAsync(ct);
            }
        }
    }
}