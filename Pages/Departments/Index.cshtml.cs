
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

namespace KibeberiUniversity.Pages.Departments
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;

        public IndexModel(IMediator mediator)
        {
            _mediator=mediator;
        }

        //data object model
        public List<DepartmentModel> Data {get;private set;}

        public async Task OnGetAsync()=>Data=await _mediator.Send(new Query());

        public class Query : IRequest<List<DepartmentModel>>
        {
        //placeholder    
        }

        public class DepartmentModel
        {
           public string Name {get;set;}

           public decimal Budget {get;set;}

           public DateTime StartDate{get;set;}
           public int Id{get;set;}

           public string AdministratorFullName{get;set;}
        }
        //mapper
        public class MappingProfile : Profile
        {
           public MappingProfile()=>CreateMap<Department,DepartmentModel>();
        }

        public class QueryHandler : IRequestHandler<Query, List<DepartmentModel>>
        {
           private readonly UniversityDbContext _dbContext;
           private readonly IConfigurationProvider _config;
          // private readonly IMapper _mapper;
          // private readonly IDbQueryFacade _queryFacade;

            public QueryHandler(UniversityDbContext context,
            IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }
            /*public QueryHandler(IDbQueryFacade queryFacade, IMapper mapper)
            {
                _queryFacade=queryFacade;
                _mapper=mapper;
            }*/

            public async Task<List<DepartmentModel>> Handle(Query query,CancellationToken ct)
            {
                return await _dbContext.Departments
                                       .ProjectTo<DepartmentModel>(_config)
                                       .ToListAsync(ct);
               /*var sql="SELECT * FROM department";
               var departments= await _queryFacade.QueryAsync<Department>(sql);
               return _mapper.Map<List<DepartmentModel>>(departments);*/
            }
        }
        
    }
}