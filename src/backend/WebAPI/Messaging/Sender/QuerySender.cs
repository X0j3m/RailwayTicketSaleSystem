using Contracts;
using Contracts.Messages.Query.Abstraction;
using MassTransit;
using WebAPI.QueueHandler.Send.Abstraction;

namespace WebAPI.QueueHandler.Send
{
    public class QuerySender : MessageSender
    {
        public QuerySender(ISendEndpointProvider sendEndpointProvider, ILogger<QuerySender> logger) : base(sendEndpointProvider, logger) { }

        public async Task SendQueryAsync<T>(T query) where T : IQuery
        {
            await SendMessageAsync(query, QueueNames.QueryQueue);
        }
    }
}