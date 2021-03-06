using System.IO;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)  // convention based 
        {
            // lifetime is scoped (for the request)
            services.AddDbContext<StoreContext>(x =>
                x.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
                //x.UseMySql(_config.GetConnectionString("DefaultConnection")));

            services.AddDbContext<AppIdentityDbContext>(x =>
            {
                x.UseSqlServer(_config.GetConnectionString("IdentityConnection"));
                //x.UseMySql(_config.GetConnectionString("IdentityConnection"));

            });

            ConfigureServices(services);
        }


        public void ConfigureProductionServices(IServiceCollection services)
        {
            services.AddDbContext<StoreContext>(x =>
            x.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
            //x.UseMySql(_config.GetConnectionString("DefaultConnection")));

            services.AddDbContext<AppIdentityDbContext>(x =>
            {
                x.UseSqlServer(_config.GetConnectionString("IdentityConnection"));
                //x.UseMySql(_config.GetConnectionString("IdentityConnection"));
            });

            ConfigureServices(services);
        }



        public void ConfigureServices(IServiceCollection services)
        {
            //OJO:  some content moved to Extensions.ApplicationServicesExtensions class
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddControllers();

            // section 13
            services.AddSingleton<IConnectionMultiplexer>(c => {
                var configuration = ConfigurationOptions.Parse(_config
                    .GetConnectionString("Redis"), true);
                return ConnectionMultiplexer.Connect(configuration);
            });

            services.AddApplicationServices();
            services.AddIdentityServices(_config);
            services.AddSwaggerDocumentation();
            services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
                });
            });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // exceptions in Development Mode
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseMiddleware<ExceptionMiddleware>();

            //by DF 51. for error handling
            // when a request is passed to the API server but we don't have an endpoint, it will redirect to our ErrorController
            app.UseStatusCodePagesWithReExecute("/errors/{0}");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            // this is for getting the images from the Content folder
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Content")),
                RequestPath = "/content"
            });

            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Content")),
            //    RequestPath = "/content"
            //});

            app.UseCors("CorsPolicy");

            app.UseAuthentication();  // see IdentityServiceExtensions.cs
            app.UseAuthorization();

            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapFallbackToController("Index", "Fallback");
            });
        }
    }
}