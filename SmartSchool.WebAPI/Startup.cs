using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartSchool.WebAPI.Data;

namespace SmartSchool.WebAPI
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

            services.AddDbContext<SmartContext>(
                context => context.UseMySql(Configuration.GetConnectionString("MySqlConnection"))
            );

            services.AddControllers()
                    .AddNewtonsoftJson(
                        Opt => Opt.SerializerSettings.ReferenceLoopHandling = 
                         Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());        


            services.AddScoped<IRepository, Repository>();

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            })
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.RegisterMiddleware = true;
            });

            var apiProviderDescription = services.BuildServiceProvider()
                                                .GetService<IApiVersionDescriptionProvider>();

            services.AddSwaggerGen(options => 
            {

                foreach (var description in apiProviderDescription.ApiVersionDescriptions)
                {
                options.SwaggerDoc(description.GroupName, new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "SmartSchool API",
                    Version = description.ApiVersion.ToString(),
                    TermsOfService = new Uri("http://termodeuso.com"),
                    Description = "A Descrição da WEBAPI",
                    License = new Microsoft.OpenApi.Models.OpenApiLicense
                    {
                        Name = "SmartSchool License",
                        Url = new Uri("http://mit.com")
                    },
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Douglas Lob",
                        Email = "",
                        Url = new Uri("http://douglaslob.com")
                    }
                });
                }
              //  var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Normalize}.xml";

            });
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider apiProviderDescription)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseSwagger()
                .UseSwaggerUI(options => 
                {
                    foreach(var description in apiProviderDescription.ApiVersionDescriptions)
                    {
                         options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", 
                                                description.GroupName.ToUpperInvariant());

                    }
                    options.RoutePrefix ="";
                });
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
