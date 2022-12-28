using AutomatonymousTest.ServiceB;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

Host.CreateDefaultBuilder(args)
    .UseSerilog((host, log) =>
    {
        log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
        log.WriteTo.Console();
    })
    .ConfigureServices(services => 
    {
        services.AddMassTransit(cfg =>
        {
            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.UsingRabbitMq((cxt, rabbitCfg) =>
            {
                rabbitCfg.Host("rabbitmq", x =>
                {
                    x.Username("guest");
                    x.Password("guest");
                });

                rabbitCfg.UseDelayedRedelivery(r => r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(5)));
                rabbitCfg.UseMessageRetry(r => r.Immediate(5));
                rabbitCfg.UseInMemoryOutbox();

                rabbitCfg.ReceiveEndpoint("service-b", e =>
                {
                    e.Consumer<DoBConsumer>();
                });

                rabbitCfg.ConfigureEndpoints(cxt);
            });
        });
    })
    .Build()
    .Run();
