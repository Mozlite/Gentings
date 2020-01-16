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
    /// ���ؽӿ����ԡ�
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
    /// �������͡�
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// ��ʼ����<see cref="Startup"/>��
        /// </summary>
        /// <param name="configuration">����ʵ����</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// ����ʵ����
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// ע�����
        /// </summary>
        /// <param name="services">���񼯺ϡ�</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddGentings(Configuration)//��ӷ����Զ�ע����
                .AddSqlServer()//���SQLServer���ݿ����
                .AddDataMigration()//������ݿ�CodeFirst�Զ�Ǩ�ƺ�̨����
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
                options.DocumentFilter<HiddenApiAttribute>(); // �ڽӿ��ࡢ����������� [HiddenApi]��������ֹ��Swagger�ĵ�������
                options.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request",
                    false); // adds any string you like to the request headers - in this case, a correlation id
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                //��api���token����֤��
                options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) ֱ�����¿�������Bearer {token}��ע������֮����һ���ո�\"",
                    Name = "Authorization", //jwtĬ�ϵĲ�������
                    In = ParameterLocation.Header, //jwtĬ�ϴ��Authorization��Ϣ��λ��(����ͷ��)
                    Type = SecuritySchemeType.ApiKey
                });
            });
            services.AddMvcCore().AddApiExplorer();
        }

        /// <summary>
        /// ����Ӧ�ùܵ���
        /// </summary>
        /// <param name="app">Ӧ�ùܵ�����ʵ����</param>
        /// <param name="env">����������</param>
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
