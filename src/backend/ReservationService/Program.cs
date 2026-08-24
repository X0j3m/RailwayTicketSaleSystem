using Contracts;
using MassTransit;
using ReservationService.QueueHandlers;
using ReservationsService.QueueHandler;

Console.WriteLine("Starting ReservationService");

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddScoped<ReservationService.Services.ReservationService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ReservationCommandConsumer>();
    x.AddConsumer<CancelReservationCommandConsumer>();
    x.AddConsumer<GetTicketsQueryConsumer>();

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

        cfg.ReceiveEndpoint(QueueNames.ReservationQueue, e =>
        {
            e.ConfigureConsumeTopology = false;

            e.UseTimeout(t => t.Timeout = TimeSpan.FromSeconds(5));

            e.ConfigureConsumer<ReservationCommandConsumer>(context);
            e.ConfigureConsumer<CancelReservationCommandConsumer>(context);
            e.ConfigureConsumer<GetTicketsQueryConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();