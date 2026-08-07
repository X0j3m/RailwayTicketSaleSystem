using Microsoft.Data.SqlClient;
using Contracts;
using MassTransit;
using ReservationsService.QueueHandler;
using System.Data;

Console.WriteLine("Starting ReservationService");

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MicrosoftSQLServer") ?? throw new ArgumentNullException("MicrosoftSQLServer");

builder.Services.AddHealthChecks();

builder.Services.AddScoped<IDbConnection>(sp => new SqlConnection(connectionString));
builder.Services.AddScoped<ReservationService.Services.ReservationService>();

builder.Services.AddScoped<ReservationCommandConsumer>();
builder.Services.AddScoped<CancelReservationCommandConsumer>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReservationCommandConsumer>();
    x.AddConsumer<CancelReservationCommandConsumer>();

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

        var queueName = QueueNames.ReservationServiceQueue;
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<ReservationCommandConsumer>(context);
            e.ConfigureConsumer<CancelReservationCommandConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();