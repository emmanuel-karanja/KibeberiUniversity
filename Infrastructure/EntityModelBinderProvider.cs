using KibeberiUniversity.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KibeberiUniversity.Infrastructure
{
    public class EntityModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return typeof(IEntity).IsAssignableFrom(context.Metadata.ModelType) ?
                 new EntityModelBinder() : null;
        }
        
    }
}