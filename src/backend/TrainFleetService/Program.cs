
using Contracts;
using MassTransit;
using Microsoft.Data.SqlClient;
using System.Data;
using TrainFleetService.QueueHandler;
using TrainFleetService.Service;

Console.WriteLine("Starting TrainFleetService");

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MicrosoftSQLServer") ?? throw new ArgumentNullException("MicrosoftSQLServer");

builder.Services.AddHealthChecks();


builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
builder.Services.AddScoped<FleetService>();

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
    options.StartTimeout = TimeSpan.FromSeconds(15);
    options.StopTimeout = TimeSpan.FromSeconds(30);
    options.ConsumerStopTimeout = TimeSpan.FromSeconds(15);
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GetAvailableSeatsQueryConsumer>();
    x.AddConsumer<GetStationsQueryConsumer>();

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

        cfg.UseTimeout(t =>
        {
            t.Timeout = TimeSpan.FromSeconds(30);
        });

        var queueName = QueueNames.TrainFleetQueue;
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureConsumeTopology = true;
            e.ConfigureConsumer<GetAvailableSeatsQueryConsumer>(context);
            e.ConfigureConsumer<GetStationsQueryConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
