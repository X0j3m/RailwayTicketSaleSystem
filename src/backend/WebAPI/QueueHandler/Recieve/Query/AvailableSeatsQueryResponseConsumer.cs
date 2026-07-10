using Contracts.Query;
using MassTransit;

namespace WebAPI.QueueHandler.Recieve.Query
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
            _logger.LogInformation("Received AvailableSeatsQueryResponse: {@Response}", response);
            await Task.CompletedTask;
        }
    }
}
