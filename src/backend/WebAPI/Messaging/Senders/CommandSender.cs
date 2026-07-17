using Contracts;
using Contracts.Messages.Backend.Command.Abstraction;
using MassTransit;
using WebAPI.Messaging.Senders.Abstraction;

namespace WebAPI.Messaging.Senders
{
    public class CommandSender : MessageSender
    {
        public CommandSender(ISendEndpointProvider sendEndpointProvider, ILogger<QuerySender> logger) : base(sendEndpointProvider, logger) { }

        public async Task SendCommandAsync<T>(T command, string queueName) where T : ICommand
        {
            await SendMessageAsync(command, queueName);
        }
    }
}
