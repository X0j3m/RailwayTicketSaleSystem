using Contracts.Messages.Frontend;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
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
            var connectionId = message.ConnectionId;
            var messageType = message.GetType();
            string messageJson = JsonSerializer.Serialize(message, messageType);
            _logger.LogInformation($"Created JSON of {messageType} type for connectionId={connectionId}");
            //_logger.LogInformation($"{messageJson}");
            await _hubContext.Clients.Client(connectionId).SendAsync(methodName, messageJson);
        }
    }
}
