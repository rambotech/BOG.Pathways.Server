using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BOG.Pathways.Server.Helpers;
using BOG.Pathways.Server.Interface;
using BOG.Pathways.Server.StorageModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace BOG.Pathways.Server
{
    /// <summary>
    ///  Standard ASP.NET Core starup
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration, IServiceProvider serviceProvider)
        {
            Configuration = configuration;
            var hostingEnv = serviceProvider.GetService<IHostingEnvironment>();
        }

        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddOptions();
            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddScoped<Security>();
            services.Configure<Settings>(Configuration);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info {
                    Version = "v1",
                    Title = "Pathways API",
                    Description = "A drop-off and pickup location for application data transfer",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "John J Schultz", Email = "", Url = "https://github.com/rambotech" },
                    License = new License { Name = "MIT", Url = "https://opensource.org/licenses/MIT" }
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlPath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "BOG.Pathways.Server.xml");
                c.IncludeXmlComments(xmlPath);
                c.DescribeAllEnumsAsStrings();
                c.DescribeStringEnumsInCamelCase();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMiddleware<IpClientWatchdogMiddleware>((MemoryStorage)serviceProvider.GetService(typeof(MemoryStorage)));

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pathways Server API v1");
            });

            app.Use((context, next) =>
            {
                context.Response.Headers.Add("X-Server-App", "BOG.Pathways.Server");
                return next();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
