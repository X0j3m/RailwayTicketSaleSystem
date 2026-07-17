using Contracts.Messages.Backend.Command;
using MassTransit;

namespace WebAPI.Messaging.Consumers.Command
{
    public class CommandResponseConsumer : IConsumer<CommandResponse>
    {
        private readonly ILogger<CommandResponseConsumer> _logger;

        public CommandResponseConsumer(ILogger<CommandResponseConsumer> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CommandResponse> context)
        {
            var response = context.Message;
            _logger.LogInformation($"Received CommandResponse: {response}");
            await Task.CompletedTask;
        }
    }
}
