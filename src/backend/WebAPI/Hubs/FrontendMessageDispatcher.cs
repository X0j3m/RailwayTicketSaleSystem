using Contracts.Messages.Frontend;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Models.Abstraction;
using System.Text.Json;
using WebAPI.Messaging.Consumers.Query;

namespace WebAPI.Hubs
{
    public class FrontendMessageDispatcher
    {
        private readonly ILogger<StationsQueryResponseConsumer> _logger;
        private readonly IHubContext<QueryHub> _hubContext;

        public FrontendMessageDispatcher(
            ILogger<StationsQueryResponseConsumer> logger,
            IHubContext<QueryHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Dispatch(string methodName, IMessage message)
        {
            string messageJson = JsonSerializer.Serialize(message, message.GetType());
            _logger.LogInformation($"Created JSON:\n{messageJson}");
            await _hubContext.Clients.All.SendAsync(methodName, messageJson);
        }
    }
}
