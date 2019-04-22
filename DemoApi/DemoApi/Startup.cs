using Jaeger;
using Jaeger.Samplers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTracing;
using OpenTracing.Util;
using System;

namespace DemoApi
{
    public class Startup
    {
        private ILoggerFactory _loggerFactory;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddOpenTracing();

            // Adds the Jaeger Tracer.
            // REFs:
            // 1) https://github.com/jaegertracing/jaeger-client-csharp
            // 2) https://stackoverflow.com/questions/51720184/how-to-connect-opentracing-application-to-a-remote-jaeger-collector
            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = serviceProvider.GetRequiredService<IHostingEnvironment>().ApplicationName;

                var samplerConfiguration = new Configuration.SamplerConfiguration(_loggerFactory)
                .WithType(ProbabilisticSampler.Type)
                .WithManagerHostPort("jaeger-agent:5778")
                .WithParam(1);

                var senderConfiguration = new Configuration.SenderConfiguration(_loggerFactory)
                    .WithAgentHost("jaeger-agent")
                    .WithAgentPort(6831);

                var reporterConfiguration = new Configuration.ReporterConfiguration(_loggerFactory)
                    .WithLogSpans(true)
                    .WithSender(senderConfiguration);
                
                Configuration config = Configuration.FromEnv(_loggerFactory);

                
                var tracer = config
                    .WithSampler(samplerConfiguration)
                    .WithReporter(reporterConfiguration)                    
                    .GetTracer();
                return tracer;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
