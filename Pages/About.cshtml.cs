using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models.UniversityViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace KibeberiUniversity.Pages
{
    public class AboutModel : PageModel
    {
        private readonly UniversityDbContext _dbContext;
        public AboutModel(UniversityDbContext context)
        {
            _dbContext=context;
        }

        public IEnumerable<EnrollmentDateGroup> Data {get;private set;}

        public async Task OnGetAsync()
        {
            var groups=await _dbContext.Students
                                       .GroupBy(x=>x.EnrollmentDate)
                                       .Select(x=> new EnrollmentDateGroup
                                        {
                                             EnrollmentDate=x.Key,
                                              StudentCount =x.Count()
                                        })
                                         .ToListAsync();
            Data=groups;
        }
    }
}