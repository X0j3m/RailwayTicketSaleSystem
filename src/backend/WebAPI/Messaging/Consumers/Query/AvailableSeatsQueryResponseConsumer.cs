using Contracts.Messages.Backend.Query;
using Contracts.Messages.Frontend;
using MassTransit;
using Models.Dto;
using WebAPI.Hubs;

namespace WebAPI.Messaging.Consumers.Query
{
    public class AvailableSeatsQueryResponseConsumer : IConsumer<AvailableSeatsQueryResponse>
    {
        private readonly ILogger<AvailableSeatsQueryResponseConsumer> _logger;
        private readonly FrontendMessageDispatcher _messageDispatcher;

        public AvailableSeatsQueryResponseConsumer(
            ILogger<AvailableSeatsQueryResponseConsumer> logger,
            FrontendMessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Consume(ConsumeContext<AvailableSeatsQueryResponse> context)
        {
            var response = context.Message;
            var connectionId = response.ConnectionId;
            List<TrainCompositionDto> messageItems = [response.Train];
            _logger.LogInformation($"Recived StationsQueryResponse for connectionId={connectionId}");
            var message = new SeatsMessage { ConnectionId = connectionId, MessageItems = messageItems };
            await _messageDispatcher.Dispatch("ReceiveAvailableSeatsQueryResponse", message);
            await Task.CompletedTask;
        }
    }
}
