
using Contracts;
using MassTransit;
using TrainFleetService.QueueHandler;

Console.WriteLine("Starting TrainFleetService");

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<GetAvailableSeatsQueryConsumer>();
builder.Services.AddScoped<GetStationsQueryConsumer>();
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

        var queueName = QueueNames.QueryQueue;
        cfg.ReceiveEndpoint(queueName, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.ConfigureConsumer<GetAvailableSeatsQueryConsumer>(context);
            e.ConfigureConsumer<GetStationsQueryConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
