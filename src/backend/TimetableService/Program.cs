using Contracts;
using MassTransit;
using TimetableService.QueueHandler;

Console.WriteLine("Starting TimetableService");

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<MessageConsumer>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();

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
            e.ConfigureConsumer<MessageConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
