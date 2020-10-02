using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KibeberiUniversity.Pages;

namespace KibeberiUniversity.Pages.Courses
{
    public class CreateModel : PageModel
    {
        private readonly IMediator _mediator;

        public CreateModel(IMediator mediator)
        {
            _mediator=mediator;
        }

        //the data model
        [BindProperty]
        public CreateCourseCommand Data{get;set;}
        public async Task<IActionResult> OnPostAsync()
        {
            await _mediator.Send(Data);
            return RedirectToPage("./Index");
        }

        public class CreateCourseCommand : IRequest<int>
        {
            [IgnoreMap]
            public int Number {get;set;}
            public string Title{get;set;}
            public int Credits{get;set;}

            public Department Department{get;set;}
        }

        public class CreateCourseMappingProfile : Profile
        {
            public CreateCourseMappingProfile()=> CreateMap<CreateCourseCommand,Course>(MemberList.Source);   
        }

        public class CreateCourseCommandHandler: IRequestHandler<CreateCourseCommand,int>
        {
            private readonly UniversityDbContext _dbContext;
            private readonly IMapper _mapper;

            public CreateCourseCommandHandler(UniversityDbContext context, IMapper mapper)
            {
                _dbContext=context;
                _mapper=mapper;
            }

            public async Task<int> Handle(CreateCourseCommand request,CancellationToken ct)
            {
                var course=_mapper.Map<CreateCourseCommand,Course>(request);
                course.Id=request.Number;

                _dbContext.Courses.Add(course);
                await _dbContext.SaveChangesAsync(ct);

                return course.Id;
            }
        }
    }
}