using FaasNet.Gateway.EF;
using FaasNet.Gateway.SqlServer.Startup.Infrastructure;
using FaasNet.Gateway.SqlServer.Startup.Infrastructure.Filters;
using FaasNet.Runtime.EF;
using FaasNet.Runtime.Serializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FaasNet.Gateway.SqlServer.Startup
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            services.AddCors(options => options.AddPolicy("AllowAll", p => p.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddGateway(opt =>
            {
                opt.FunctionApi = Configuration["KubernetesApi"];
                opt.PromotheusApi = Configuration["PromotheusApi"];
                opt.PrometheusFilePath = Configuration["PrometheusFilePath"];
            }, opt =>
            {
                opt.AccountId = 678128;
                opt.LicenseKey = "gubXjBR4DMjsdqw3";
                opt.Host = "geolite.info";
            })
                .AddRuntimeEF(opt =>
                {
                    opt.UseLazyLoadingProxies();
                    opt.UseSqlServer(Configuration.GetConnectionString("Runtime"), o => o.MigrationsAssembly(migrationsAssembly));
                })
                .AddGatewayEF(opt =>
                {
                    opt.UseSqlServer(Configuration.GetConnectionString("Runtime"), o => o.MigrationsAssembly(migrationsAssembly));
                });
            services.AddSwaggerGen();
            services.AddControllers(opts =>
            {
                opts.InputFormatters.Add(new YamlInputFormatter());
                opts.OutputFormatters.Add(new YamlOutputFormatter());
                opts.Filters.Add(new FaasNetExceptionFilter());
                opts.RespectBrowserAcceptHeader = true;
            }).AddNewtonsoftJson(opts =>
            {
                opts.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway V1");
            });
            app.UseCors("AllowAll");
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = scope.ServiceProvider.GetService<RuntimeDBContext>())
                {
                    context.Database.Migrate();
                    if (!context.WorkflowDefinitions.Any())
                    {
                        var files = Directory.EnumerateFiles(Path.Combine(Directory.GetCurrentDirectory(), "ServerlessWorkflows"), "*.yml").ToList();
                        var runtimeSerializer = new RuntimeSerializer();
                        foreach (var file in files)
                        {
                            var workflowDef = runtimeSerializer.DeserializeYaml(File.ReadAllText(file));
                            workflowDef.RefreshTechnicalId();
                            workflowDef.CreateDateTime = DateTime.UtcNow;
                            workflowDef.UpdateDateTime = DateTime.UtcNow;
                            workflowDef.IsLast = true;
                            context.WorkflowDefinitions.Add(workflowDef);
                        }

                    }

                    context.SaveChanges();
                }

                using (var context = scope.ServiceProvider.GetService<GatewayDBContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
