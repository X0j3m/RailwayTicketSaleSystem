
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

        var queueName = QueueNames.TrainFleetQueue;
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureConsumeTopology = true;
            e.ConfigureConsumer<GetAvailableSeatsQueryConsumer>(context);
            e.ConfigureConsumer<GetStationsQueryConsumer>(context);
            e.UseTimeout(t => t.Timeout = TimeSpan.FromSeconds(10));
        });
    });
});

var host = builder.Build();
host.Run();
