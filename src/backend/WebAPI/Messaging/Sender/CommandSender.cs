using Contracts;
using Contracts.Messages.Command.Abstraction;
using MassTransit;
using WebAPI.Messaging.Sender.Abstraction;

namespace WebAPI.Messaging.Sender
{
    public class CommandSender : MessageSender
    {
        public CommandSender(ISendEndpointProvider sendEndpointProvider, ILogger<QuerySender> logger) : base(sendEndpointProvider, logger) { }

        public async Task SendCommandAsync<T>(T command) where T : ICommand
        {
            await SendMessageAsync(command, QueueNames.CommandQueue);
        }
    }
}
