using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using KibeberiUniversity.DataContext;
using KibeberiUniversity.Behaviors;
using KibeberiUniversity.Infrastructure;
using KibeberiUniversity.Infrastructure.Tags;
using MediatR;
using AutoMapper;
using HtmlTags;
using FluentValidation.AspNetCore;
using KibeberiUniversity.Utils;


namespace KibeberiUniversity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMiniProfiler().AddEntityFramework();
            services.AddDbContext<UniversityDbContext>( options => 
             options.UseMySql(Configuration.GetConnectionString("DefaultConnection")
              ));
            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(Startup));

            services.AddScoped(typeof(IPipelineBehavior<,>),typeof(TransactionBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>),typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>),typeof(RequestPerformanceBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>),typeof(RequestCachingBehavior<,>));

            //services.AddScoped(typeof(IByteSerializer<>),typeof(ByteSerializer<>));
            services.AddScoped<IDbQueryFacade,DapperDbQueryFacade>();


            services.AddHtmlTags(new TagConventions());
            services.AddRazorPages(opt =>
                {
                    opt.Conventions.ConfigureFilter(new DbContextTransactionPageFilter());
                    opt.Conventions.ConfigureFilter(new ValidatorPageFilter());
                })
                .AddFluentValidation(cfg => { cfg.RegisterValidatorsFromAssemblyContaining<Startup>(); });

           services.AddMvc( opt=> opt.ModelBinderProviders.Insert(0,new EntityModelBinderProvider()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiniProfiler();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
