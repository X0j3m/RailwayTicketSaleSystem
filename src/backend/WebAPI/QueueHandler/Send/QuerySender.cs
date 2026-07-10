using Contracts;
using Contracts.Query.Abstraction;
using MassTransit;

namespace WebAPI.QueueHandler.Send
{
    public class QuerySender
    {
        private readonly ILogger<QuerySender> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public QuerySender(ISendEndpointProvider sendEndpointProvider, ILogger<QuerySender> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task SendQueryAsync<T>(T query) where T : IQuery
        {
            var queueUri = new Uri($"queue:{QueueNames.QueryQueue}");
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(queueUri);

            await endpoint.Send(query);
            _logger.LogInformation($"Query of type {typeof(T).Name} sent to queue {QueueNames.QueryQueue}");
        }
    }
}