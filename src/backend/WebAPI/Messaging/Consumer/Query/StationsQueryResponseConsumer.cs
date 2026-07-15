using Contracts.Messages.Query;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Hub;

namespace WebAPI.Messaging.Consumer.Query
{
    public class StationsQueryResponseConsumer : IConsumer<StationsQueryResponse>
    {
        private readonly ILogger<StationsQueryResponseConsumer> _logger;
        private readonly IHubContext<FrontendHub> _hubContext;

        public StationsQueryResponseConsumer(
            ILogger<StationsQueryResponseConsumer> logger,
            IHubContext<FrontendHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<StationsQueryResponse> context)
        {
            var response = context.Message;
            _logger.LogInformation($"Received response: {string.Join(", ", response.Stations)}");
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", string.Join(", ", response.Stations));
            await Task.CompletedTask;
        }
    }
}
