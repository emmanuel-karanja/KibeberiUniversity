using System.Collections.Generic;
using KibeberiUniversity.DataContext;
using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;

namespace KibeberiUniversity.Infrastructure.Tags
{
    public abstract class EntitySelectElementBuilder<T> : ElementTagBuilder where T: class
    {
        public override bool Matches(ElementRequest subject)
        {
            return typeof(T).IsAssignableFrom(subject.Accessor.PropertyType);
        }

        public override HtmlTag Build(ElementRequest request)
        {
            var results=Source(request);

            var selectTag=new SelectTag(t=>
            {
                t.Option(string.Empty,string.Empty);
                foreach(var result in results)
                {
                    BuildOptionsTag(t,result,request);
                }
            });

            var entity=request.Value<T>();
            
            if(entity != null)
            {
                selectTag.SelectByValue(GetValue(entity));
            }

            return selectTag;
        }

        protected virtual HtmlTag BuildOptionsTag(SelectTag select, T model, ElementRequest request)
        {
            return select.Option(GetDisplayValue(model),GetValue(model));
        }

        protected abstract int GetValue(T instance);
        protected abstract string GetDisplayValue(T instance);

        protected virtual IEnumerable<T> Source(ElementRequest request)
        {
            return request.Get<UniversityDbContext>().Set<T>();
        }
    }
}