using EventMesh.Runtime;
using EventMesh.Runtime.EF;
using EventMesh.Runtime.MessageBroker;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;

namespace EventMesh.Runtime.Website
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
            var path = Path.Combine(Environment.CurrentDirectory, "Runtime.db");
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHostedService<RuntimeHostedService>();
            RegisterInMemoryEventMeshServer(services).AddRuntimeEF(opt => opt.UseSqlite($"Data Source={path}", optionsBuilders => optionsBuilders.MigrationsAssembly(migrationsAssembly)));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            Migrate(app.ApplicationServices);
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        private static IServiceCollection RegisterInMemoryEventMeshServer(IServiceCollection services)
        {
            return services.AddRuntimeWebsite(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            }).AddInMemoryMessageBroker(new ConcurrentBag<InMemoryTopic>());
        }

        private static IServiceCollection RegisterAMQPEventMeshServer(IServiceCollection services)
        {
            return services.AddRuntimeWebsite(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            }).AddAMQP();
        }

        private static IServiceCollection RegisterKafkaEventMeshServer(IServiceCollection services)
        {
            return services.AddRuntimeWebsite(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            }).AddKafka();
        }

        private static IServiceCollection RegisterFullEventMeshServer(IServiceCollection services)
        {
            return services.AddRuntimeWebsite(opt =>
            {
                opt.Urn = "localhost";
                opt.Port = 4000;
            }).AddAMQP().AddKafka();
        }

        private static void Migrate(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<EventMeshDBContext>();
                dbContext.Database.Migrate();
                scope.ServiceProvider.SeedAMQPOptions();
                scope.ServiceProvider.SeedKafkaOptions();
            }
        }
    }
}
