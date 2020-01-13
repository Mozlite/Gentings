using System.IO;
using Gentings;
using Gentings.Data.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace Genting.Apis
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
            services.AddMemoryCache();
            services.AddGentings(Configuration)
                .AddSqlServer();
            services.AddSwaggerGen(c =>
            {

                c.SwaggerDoc("v1", new Info { Title = "Hello", Version = "v1" });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;

                var xmlPath = Path.Combine(basePath, "WebApplication2.xml");

                c.IncludeXmlComments(xmlPath);

            });
            services.AddMvcCore().AddApiExplorer();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseSwaggerUI(c => {
                c.ShowExtensions();
                c.EnableValidator();
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "test V1");
                c.RoutePrefix = "swagger";
            });
            app.UseSwagger();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
