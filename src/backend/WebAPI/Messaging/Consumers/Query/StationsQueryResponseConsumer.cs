using Contracts.Messages.Backend.Query;
using Contracts.Messages.Frontend;
using MassTransit;
using WebAPI.Hubs;

namespace WebAPI.Messaging.Consumers.Query
{
    public class StationsQueryResponseConsumer : IConsumer<StationsQueryResponse>
    {
        private readonly ILogger<StationsQueryResponseConsumer> _logger;
        private readonly FrontendMessageDispatcher _messageDispatcher;

        public StationsQueryResponseConsumer(
            ILogger<StationsQueryResponseConsumer> logger,
            FrontendMessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Consume(ConsumeContext<StationsQueryResponse> context)
        {
            var response = context.Message;
            var connectionId = response.ConnectionId;
            _logger.LogInformation($"Received StationsQueryResponse for connectionId={connectionId}");
            var message = new StationsMessage { ConnectionId = connectionId, MessageItems = response.Stations };
            await _messageDispatcher.Dispatch("ReceiveStationsQueryResponse", message);
            await Task.CompletedTask;
        }
    }
}
