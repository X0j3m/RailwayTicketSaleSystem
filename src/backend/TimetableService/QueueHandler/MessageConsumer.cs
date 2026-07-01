using MassTransit;
using Contracts.Query;

namespace TimetableService.QueueHandler
{
    public class MessageConsumer : IConsumer<IQuery>
    {
        private readonly ILogger<MessageConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;

        public MessageConsumer(ILogger<MessageConsumer> logger, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<IQuery> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Received query: {message.GetType().Name}");

            await Task.Delay(3000);

            await _endpoint.Publish<IQueryResponse>(
                new StationsQueryResponse
                {
                    Stations = new List<string>
                    {
                        "Station A",
                        "Station B",
                        "Station C"
                    }
                });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
