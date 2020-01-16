using System;
using System.IO;
using System.Text;
using Gentings.Data.Migrations;
using Gentings.Data.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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

        private string GetConfig(string key) => Configuration.GetSection(key)?.Value;

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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = GetConfig("Jwt:Issuer"),
                        ValidAudience = GetConfig("Jwt:Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetConfig("Jwt:SecurityKey")))
                    };
                });
            services.AddAuthorization();
            services.AddSwaggerGen(options =>
            {
                string contactName = GetConfig("SwaggerDoc:Contact:Name");
                string contactNameEmail = GetConfig("SwaggerDoc:Contact:Email");
                string contactUrl = GetConfig("SwaggerDoc:Contact:Url");
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = GetConfig("SwaggerDoc:Version"),
                    Title = GetConfig("SwaggerDoc:Title"),
                    Description = GetConfig("SwaggerDoc:Description"),
                    Contact = new OpenApiContact
                    { Name = contactName, Email = contactNameEmail, Url = new Uri(contactUrl) },
                    License = new OpenApiLicense { Name = contactName, Url = new Uri(contactUrl) }
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
            app.UseGentings(Configuration);

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
