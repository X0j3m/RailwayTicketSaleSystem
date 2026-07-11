using Contracts.Messages.Query;
using MassTransit;

namespace WebAPI.QueueHandler.Recieve.Query
{
    public class TrainConnectionsQueryResponseConsumer : IConsumer<TrainConnectionsQueryResponse>
    {
        private readonly ILogger<TrainConnectionsQueryResponseConsumer> _logger;

        public TrainConnectionsQueryResponseConsumer(ILogger<TrainConnectionsQueryResponseConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<TrainConnectionsQueryResponse> context)
        {
            var response = context.Message;
            _logger.LogInformation($"Received response: {string.Join(", ", response.Connections)}");
            await Task.CompletedTask;
        }
    }
}
