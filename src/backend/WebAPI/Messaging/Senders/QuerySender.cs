using Contracts;
using Contracts.Messages.Backend.Query.Abstraction;
using MassTransit;
using WebAPI.Messaging.Senders.Abstraction;

namespace WebAPI.Messaging.Senders
{
    public class QuerySender : MessageSender
    {
        public QuerySender(ISendEndpointProvider sendEndpointProvider, ILogger<QuerySender> logger) : base(sendEndpointProvider, logger) { }

        public async Task SendQueryAsync<T>(T query, string queueName) where T : IQuery
        {
            await SendMessageAsync(query, queueName);
        }
    }
}