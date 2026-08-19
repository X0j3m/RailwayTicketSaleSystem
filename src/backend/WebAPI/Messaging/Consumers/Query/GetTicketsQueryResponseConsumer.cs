using Contracts.Messages.Backend.Query;
using Contracts.Messages.Frontend;
using MassTransit;
using WebAPI.Hubs.Utils;

namespace WebAPI.Messaging.Consumers.Query
{
    public class GetTicketsQueryResponseConsumer : IConsumer<GetTicketsQueryResponse>
    {
        private readonly ILogger<GetTicketsQueryResponseConsumer> _logger;
        private readonly MessageDispatcher _messageDispatcher;

        public GetTicketsQueryResponseConsumer(
            ILogger<GetTicketsQueryResponseConsumer> logger,
            MessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Consume(ConsumeContext<GetTicketsQueryResponse> context)
        {
            var response = context.Message;
            var connectionId = response.ConnectionId;
            _logger.LogInformation("Received GetTicketsQueryResponse for ConnectionId: {ConnectionId}", connectionId);
            var message = new TicketsMessage { ConnectionId = connectionId, MessageItems = response.Tickets };
            await _messageDispatcher.Dispatch("ReceiveGetTicketsQueryResponse", message);
            await Task.CompletedTask;
        }
    }
}
