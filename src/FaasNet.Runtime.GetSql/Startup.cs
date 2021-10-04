﻿using FaasNet.Runtime.Factories;
using FaasNet.Runtime.GetSql.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Prometheus;
using System.IO;

namespace FaasNet.Runtime.GetSql
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MetricReporter>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMetricServer();
            app.Map("/configuration", (c) =>
            {
                c.Run(async (httpContext) =>
                {
                    var configuration = ConfigurationFactory.New(typeof(GetSqlConfiguration));
                    var json = JsonConvert.SerializeObject(configuration);
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.ContentType = "application/json";
                    await httpContext.Response.WriteAsync(json);
                });
            });
            app.UseMiddleware<ResponseMetricMiddleware>();
            app.UseMiddleware<FunctionMiddleware>();
        }
    }
}
