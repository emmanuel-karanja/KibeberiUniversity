using System.Threading.Tasks;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace KibeberiUniversity.Infrastructure
{
    public class EntityModelBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var original= bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if(original != ValueProviderResult.None)
            {
                var originalValue = original.FirstValue;
                if(int.TryParse(originalValue, out var id))
                {
                    var dbContext= bindingContext.HttpContext.RequestServices.GetService<UniversityDbContext>();
                    IEntity entity=null;
                    if(bindingContext.ModelType == typeof(Course))
                    {
                        entity=await dbContext.Set<Course>().FindAsync(id);
                    }
                    else if (bindingContext.ModelType == typeof(Department))
                    {
                        entity=await dbContext.Set<Department>().FindAsync(id);
                    }
                    else if (bindingContext.ModelType == typeof(Enrollment))
                    {
                       entity= await dbContext.Set<Enrollment>().FindAsync(id);   
                    }
                    else if (bindingContext.ModelType == typeof(Instructor))
                    {
                        entity=await dbContext.Set<Instructor>().FindAsync(id);
                    }
                    else if (bindingContext.ModelType == typeof(Student))
                    {
                        entity=await dbContext.Set<Student>().FindAsync(id);
                    }

                    bindingContext.Result=entity != null ? ModelBindingResult.Success(entity) : bindingContext.Result;
                }
            }
        }
    }
}