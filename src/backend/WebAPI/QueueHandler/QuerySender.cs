using Contracts;
using Contracts.Query;
using MassTransit;

namespace WebAPI.QueueSender
{
    public class QuerySender
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public QuerySender(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task SendQueryAsync(IQuery query)
        {
            var queueName = QueueNames.QueryQueue;
            var queueUri = new Uri($"queue:{queueName}");
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(queueUri);

            await endpoint.Send(query);
        }
    }
}
