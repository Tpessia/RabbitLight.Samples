using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitLight.AspNetCore.Sample.Consumers.Context;
using RabbitLight.Config;
using RabbitLight.Extensions;
using RabbitLight.Publisher;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace RabbitLight.AspNetCore.Sample
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
            services.AddLogging(opt => opt.AddConsole(c => c.TimestampFormat = "[HH:mm:ss.fff] "));

            services.AddControllers();

            services.AddRabbitLightContext<AspNetAppContext>(config =>
            {
                config.Alias = nameof(AspNetAppContext);

                config.UseHostedService = true;

                config.ConnConfig = ConnectionConfig.FromConfig(Configuration.GetSection("RabbitLight"));

                config.Consumers = Assembly.GetEntryAssembly().GetTypes();

                config.OnConfig = async (sp) =>
                {
                    var context = sp.GetRequiredService<AspNetAppContext>();
                    await context.Api.CreateExchange("manual-exchange");
                    await context.Api.CreateQueue("manual-queue");
                    await context.Api.CreateBind("manual-queue", "manual-exchange", "manual-key");
                };

                config.OnStart = (sp, consumer, ea) => Task.Run(() =>
                    consumer.Logger.LogInformation($"Starting {consumer.Type.Name}: {ea.DeliveryTag}"));

                config.OnEnd = (sp, consumer, ea) => Task.Run(() =>
                    consumer.Logger?.LogInformation($"Ending {consumer.Type.Name}: {ea.DeliveryTag}"));

                config.OnAck = (sp, consumer, ea) => Task.Run(() =>
                    consumer.Logger?.LogInformation($"Acked {consumer.Type.Name}: {ea.DeliveryTag}"));

                config.OnError = (sp, consumer, ea, ex) => Task.Run(() =>
                {
                    consumer.Logger?.LogError($"Handled error in {consumer.Type.Name}: {ea.DeliveryTag}");

                    // Requeue if the queue doesn't have a Dead Letter as fallback
                    var requeue = !(consumer?.Queue?.Arguments?.ContainsKey("x-dead-letter-exchange")).GetValueOrDefault();

                    var requeueDelay = config.ConnConfig.RequeueDelay ?? TimeSpan.FromSeconds(30);
                    return requeue ? requeueDelay : default(TimeSpan?);
                });

                config.OnPublisherNack = (sp, sender, ea) => Task.Run(() =>
                {
                    var logger = sp.GetService<ILoggerFactory>()?.CreateLogger<IPublisher>();
                    logger?.LogError($"Publisher error: {ea.DeliveryTag}");
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
