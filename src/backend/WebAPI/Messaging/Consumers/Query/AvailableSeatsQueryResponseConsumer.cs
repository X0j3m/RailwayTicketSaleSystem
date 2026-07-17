using Contracts.Messages.Backend.Query;
using MassTransit;

namespace WebAPI.Messaging.Consumers.Query
{
    public class AvailableSeatsQueryResponseConsumer : IConsumer<AvailableSeatsQueryResponse>
    {
        private readonly ILogger<AvailableSeatsQueryResponseConsumer> _logger;

        public AvailableSeatsQueryResponseConsumer(ILogger<AvailableSeatsQueryResponseConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<AvailableSeatsQueryResponse> context)
        {
            var response = context.Message;
            _logger.LogInformation($"Received AvailableSeatsQueryResponse: {response}");
            await Task.CompletedTask;
        }
    }
}
