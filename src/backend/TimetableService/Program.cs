using Contracts;
using MassTransit;
using Neo4j.Driver;
using TimetableService.QueueHandler;
using TimetableService.Services;

Console.WriteLine("Starting TimetableService");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddScoped<GetTrainConnectionsQueryConsumer>();
builder.Services.AddScoped<ScheduleService>();

builder.Services.AddSingleton(sp =>
{
    var uri = builder.Configuration.GetValue<string>("Neo4j:Uri") ?? throw new ArgumentNullException("Neo4j:Uri");
    var login = builder.Configuration.GetValue<string>("Neo4j:Login") ?? throw new ArgumentNullException("Neo4j:Login");
    var password = builder.Configuration.GetValue<string>("Neo4j:Password") ?? throw new ArgumentNullException("Neo4j:Password");

    return GraphDatabase.Driver(uri, AuthTokens.Basic(login, password));
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GetTrainConnectionsQueryConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var uri = builder.Configuration.GetValue<string>("RabbitMQ:Uri") ?? throw new ArgumentNullException("RabbitMQ:Uri");
        var login = builder.Configuration.GetValue<string>("RabbitMQ:Login") ?? throw new ArgumentNullException("RabbitMQ:Login");
        var password = builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? throw new ArgumentNullException("RabbitMQ:Password");

        cfg.Host(new Uri(uri), h =>
        {
            h.Username(login);
            h.Password(password);
        });

        var queueName = QueueNames.TimetableServiceQueue;
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<GetTrainConnectionsQueryConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
