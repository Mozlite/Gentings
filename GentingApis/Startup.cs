using System;
using System.IO;
using Gentings.Data.Migrations;
using Gentings.Data.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gentings.Apis
{
    /// <summary>
    /// 隐藏接口特性。
    /// </summary>
    public class HiddenApiAttribute : Attribute, IDocumentFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="swaggerDoc"></param>
        /// <param name="context"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

        }
    }

    /// <summary>
    /// 启动类型。
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 初始化类<see cref="Startup"/>。
        /// </summary>
        /// <param name="configuration">配置实例。</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 配置实例。
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 注册服务。
        /// </summary>
        /// <param name="services">服务集合。</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddGentings(Configuration)//添加服务自动注册框架
                .AddSqlServer()//添加SQLServer数据库服务
                .AddDataMigration()//添加数据库CodeFirst自动迁移后台服务
                ;
            services.AddAuthentication();
            services.AddAuthorization();
            services.AddSwaggerGen(options =>
            {
                string contactName = Configuration.GetSection("SwaggerDoc:Contact:Name").Value;
                string contactNameEmail = Configuration.GetSection("SwaggerDoc:Contact:Email").Value;
                string contactUrl = Configuration.GetSection("SwaggerDoc:Contact:Url").Value;
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = Configuration.GetSection("SwaggerDoc:Version").Value,
                    Title = Configuration.GetSection("SwaggerDoc:Title").Value,
                    Description = Configuration.GetSection("SwaggerDoc:Description").Value,
                    Contact = new OpenApiContact
                        {Name = contactName, Email = contactNameEmail, Url = new Uri(contactUrl)},
                    License = new OpenApiLicense {Name = contactName, Url = new Uri(contactUrl)}
                });

                var basePath = Directory.GetCurrentDirectory();
                var xmlPath = Path.Combine(basePath, typeof(Startup).Assembly.GetName().Name + ".xml");
                options.IncludeXmlComments(xmlPath);
                options.DocumentFilter<HiddenApiAttribute>(); // 在接口类、方法标记属性 [HiddenApi]，可以阻止【Swagger文档】生成
                options.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request",
                    false); // adds any string you like to the request headers - in this case, a correlation id
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                //给api添加token令牌证书
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                    Name = "Authorization", //jwt默认的参数名称
                    In = ParameterLocation.Header, //jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
            });
            services.AddMvcCore().AddApiExplorer();
        }

        /// <summary>
        /// 配置应用管道。
        /// </summary>
        /// <param name="app">应用管道构建实例。</param>
        /// <param name="env">环境变量。</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHelp V1");
                options.RoutePrefix = string.Empty;
            });
        }
    }
}
