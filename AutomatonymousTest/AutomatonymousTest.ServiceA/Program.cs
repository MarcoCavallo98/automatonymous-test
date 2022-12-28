using AutomatonymousTest.ServiceA.Sagas.StateMachines;
using AutomatonymousTest.ServiceA.Sagas.States;
using MassTransit;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((host, log) => 
{
    log.MinimumLevel.Override("Microsoft", LogEventLevel.Warning);
    log.WriteTo.Console();
});

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(cfg =>
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

        rabbitCfg.ConfigureEndpoints(cxt);
    });
    cfg.AddSagaStateMachine<TestStateMachine, TestState>()
        .MongoDbRepository(x =>
        {
            x.Connection = "mongodb://mongodb";
            x.DatabaseName = "testsagadb";
        })
        .Endpoint(x => 
        {
            x.Name = "test-saga";
            x.ConcurrentMessageLimit = 8;
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
