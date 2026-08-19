using Contracts.Messages.Backend.Query;
using Contracts.Messages.Frontend;
using MassTransit;
using WebAPI.Hubs.Utils;

namespace WebAPI.Messaging.Consumers.Query
{
    public class AvailableSeatsQueryResponseConsumer : IConsumer<AvailableSeatsQueryResponse>
    {
        private readonly ILogger<AvailableSeatsQueryResponseConsumer> _logger;
        private readonly MessageDispatcher _messageDispatcher;

        public AvailableSeatsQueryResponseConsumer(
            ILogger<AvailableSeatsQueryResponseConsumer> logger,
            MessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Consume(ConsumeContext<AvailableSeatsQueryResponse> context)
        {
            var response = context.Message;
            var connectionId = response.ConnectionId;
            _logger.LogInformation($"Received AvailableSeatsQueryResponse for ConnectionId={connectionId}");
            var message = new SeatsMessage { ConnectionId = connectionId, MessageItems = response.Trains };
            await _messageDispatcher.Dispatch("ReceiveAvailableSeatsQueryResponse", message);
            await Task.CompletedTask;
        }
    }
}
