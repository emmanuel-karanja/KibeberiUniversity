
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
using KibeberiUniversity.Utils;

namespace KibeberiUniversity.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly IMediator _mediator;
        public IndexModel(IMediator mediator)=> _mediator=mediator;

        public Result Data {get;private set;}

        public async Task OnGetAsync(string sortOrder,string currentFilter,string searchString, int? pageIndex)
        {
           Data= await _mediator.Send(new Query{
                CurrentFilter=currentFilter, 
                Page=pageIndex,
                SearchString=searchString,
                SortOrder=sortOrder
           });     
        }

        public class Query: IRequest<Result>
        {
            public string SortOrder{get;set;}
            public string SearchString{get;set;}
            public string CurrentFilter{get;set;}
            public int? Page{get;set;}
        }

        public class Result
        {
            public string CurrentSort{get;set;}
            public string NameSortParam{get;set;}
            public string DateSortParam{get;set;}
            public string CurrentFilter{get;set;}
            public string SearchString{get;set;}

            public PaginatedList<Model> Results{get;set;}
        }

        public class Model 
        {
            public int Id{get;set;}
            [Display(Name="First Name")]
            public string FirstMidName{get;set;}
            public string LastName {get;set;}
            public DateTime EnrollmentDate{get;set;}
            public int EnrollmentsCount{get;set;}
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()=>CreateMap<Student,Model>();
        }

        public class QueryHandler : IRequestHandler<Query,Result>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IConfigurationProvider _config;

            public QueryHandler(UniversityDbContext context, IConfigurationProvider config)
            {
                _dbContext=context;
                _config=config;
            }

            public async Task<Result> Handle(Query query, CancellationToken ct)
            {
                var model=new Result{
                    CurrentSort=query.SortOrder,
                    NameSortParam=String.IsNullOrEmpty(query.SortOrder) ? "name_desc" :"",
                    DateSortParam=query.SortOrder== "Date" ? "date_desc" :  "Date"
                };

                if(query.SearchString !=null)
                {
                    query.Page=1;
                }
                else
                {
                    query.SearchString=query.CurrentFilter;
                }

                model.CurrentFilter=query.SearchString;
                model.SearchString=query.SearchString;

                IQueryable<Student> students= _dbContext.Students;
                if(!String.IsNullOrEmpty(query.SearchString))
                {
                    students=students.Where(s=>s.LastName.Contains(query.SearchString) 
                                              || s.FirstMidName.Contains(query.SearchString));
                }

                switch(query.SortOrder)
                {
                    case "name_desc":
                       students=students.OrderByDescending(s=>s.LastName);
                       break;
                    case "Date":
                      students=students.OrderBy(s=>s.EnrollmentDate);
                      break;
                    case "date_desc":
                      students=students.OrderByDescending(s=>s.EnrollmentDate);
                      break;
                    default:
                      students=students.OrderBy(s=>s.LastName);
                      break;
                }
 
             int pageSize=10;
             int pageNumber=query.Page ?? 1;
             model.Results=await students.ProjectTo<Model>(_config)
                                         .PaginatedListAsync(pageNumber,pageSize);

            return model;
            }
        }
    }
}