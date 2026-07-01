using Contracts.Query;
using MassTransit;

namespace WebAPI.QueueHandler
{
    public class ResponseConsumer : IConsumer<IQueryResponse>
    {
        private readonly ILogger<ResponseConsumer> _logger;

        public ResponseConsumer(ILogger<ResponseConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IQueryResponse> context)
        {
            var response = context.Message;
            _logger.LogInformation($"Received response: {response.GetType().Name}");
            await Task.CompletedTask;
        }
    }
}
