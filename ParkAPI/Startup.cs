using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ParkAPI.Models;
using ParkAPI.ParkMapper;
using ParkAPI.Repository;
using ParkAPI.Repository.IRepository;
using System;
using System.IO;
using System.Reflection;
using WebApplication.Extensions;

namespace ParkAPI
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
            services.AddJWTTokenServices(Configuration);
            services.AddControllers();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefConnection")));
            services.AddScoped<INationalParkRepository, NationalParkRepository>();

            //for implementing swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("ParkAPISpec",
                    new Microsoft.OpenApi.Models.OpenApiInfo()
                    {
                        Title = "Park API",
                        Version = "1",
                        Description = "Chetu Park API",
                        Contact = new Microsoft.OpenApi.Models.OpenApiContact()
                        {
                            Email = "architgoel9596@gmail.com",
                            Name = "",
                            Url = new Uri("https://www.google.com")
                        },
                        License = new Microsoft.OpenApi.Models.OpenApiLicense()
                        {
                            Name = "MIT License",
                            Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
                        }
                    });
                //for implementation of xml comments
                var xmlCommentFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var cmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentFile);
                options.IncludeXmlComments(cmlCommentsFullPath);

                //FOR Implementation of Authorization
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                     {
                         new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                         {
                             Reference = new Microsoft.OpenApi.Models.OpenApiReference
                             {
                                  Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                  Id = "Bearer"
                             }
                         },
                         new string[] {}
                     }
                });
            });

            //for implementing automapper
            services.AddAutoMapper(typeof(ParkMappings));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            //for implementing swagger
            app.UseSwagger();
            app.UseSwaggerUI(options => {
                options.SwaggerEndpoint("/swagger/ParkAPISpec/swagger.json", "Park API");
                options.RoutePrefix="";
            });
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
