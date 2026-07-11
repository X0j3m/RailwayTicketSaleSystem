using Contracts.Messages;
using MassTransit;

namespace WebAPI.QueueHandler.Send.Abstraction
{
    public abstract class MessageSender
    {
        private readonly ILogger<QuerySender> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public MessageSender(ISendEndpointProvider sendEndpointProvider, ILogger<QuerySender> logger)
        {
            _sendEndpointProvider = sendEndpointProvider;
            _logger = logger;
        }

        public async Task SendMessageAsync<T>(T message, string queueName) where T : IMessage
        {
            var queueUri = new Uri($"queue:{queueName}");
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(queueUri);

            await endpoint.Send(message);
            _logger.LogInformation($"Message of type {typeof(T).Name} sent to queue {queueName}");
        }
    }
}
