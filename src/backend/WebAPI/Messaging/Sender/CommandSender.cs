using Contracts;
using Contracts.Messages.Command.Abstraction;
using MassTransit;
using WebAPI.QueueHandler.Send.Abstraction;

namespace WebAPI.QueueHandler.Send
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
