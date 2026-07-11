using Contracts.Messages.Query;
using MassTransit;

namespace WebAPI.QueueHandler.Recieve.Query
{
    public class StationsQueryResponseConsumer : IConsumer<StationsQueryResponse>
    {
        private readonly ILogger<StationsQueryResponseConsumer> _logger;

        public StationsQueryResponseConsumer(ILogger<StationsQueryResponseConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StationsQueryResponse> context)
        {
            var response = context.Message;
            _logger.LogInformation($"Received response: {string.Join(", ", response.Stations)}");
            await Task.CompletedTask;
        }
    }
}
