using Contracts.Messages.Backend.Query;
using Contracts.Messages.Frontend;
using MassTransit;
using WebAPI.Hubs;

namespace WebAPI.Messaging.Consumers.Query
{
    public class TrainConnectionsQueryResponseConsumer : IConsumer<TrainConnectionsQueryResponse>
    {
        private readonly ILogger<TrainConnectionsQueryResponseConsumer> _logger;
        private readonly FrontendMessageDispatcher _messageDispatcher;

        public TrainConnectionsQueryResponseConsumer(
            ILogger<TrainConnectionsQueryResponseConsumer> logger,
            FrontendMessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Consume(ConsumeContext<TrainConnectionsQueryResponse> context)
        {
            var response = context.Message;
            var connectionId = response.ConnectionId;
            _logger.LogInformation($"Recived TrainConnectionsQueryResponse for connectionId={connectionId}");
            var message = new TrainConnectionsMessage { ConnectionId = connectionId, MessageItems = response.Connections };
            await _messageDispatcher.Dispatch("ReceiveTrainConnectionsQueryResponse", message);
            await Task.CompletedTask;
        }
    }
}
