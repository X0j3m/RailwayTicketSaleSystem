using MassTransit;
using Scalar.AspNetCore;
using WebAPI.Hubs;
using WebAPI.Hubs.Utils;
using WebAPI.Messaging.Consumers.Command;
using WebAPI.Messaging.Consumers.Query;
using WebAPI.Messaging.Senders;

Console.WriteLine("Starting WebAPI");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    var uri = builder.Configuration.GetValue<string>("Frontend:Uri") ?? throw new ArgumentNullException("Frontend:Uri");

    options.AddPolicy("SignalRPolicy",
        policy =>
        {
            policy.WithOrigins(uri)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<StationsQueryResponseConsumer>();
    x.AddConsumer<TrainConnectionsQueryResponseConsumer>();
    x.AddConsumer<AvailableSeatsQueryResponseConsumer>();
    x.AddConsumer<CommandResponseConsumer>();

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
        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.AddScoped<QuerySender>();
builder.Services.AddScoped<CommandSender>();

builder.Services.AddScoped<MessageDispatcher>();

var app = builder.Build();

app.UseCors("SignalRPolicy");
app.UseRouting();
app.UseAuthorization();

app.MapHub<MessageHub>("/hub/app");
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

//app.UseHttpsRedirection();

//app.MapGet("/", () => "WebAPI is running properly");

app.Run();