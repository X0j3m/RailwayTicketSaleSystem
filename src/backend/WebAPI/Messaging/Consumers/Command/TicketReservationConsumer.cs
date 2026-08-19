using Contracts.Messages.Backend.Command;
using Contracts.Messages.Frontend;
using MassTransit;
using Models.Dtos;
using WebAPI.Hubs.Utils;

namespace WebAPI.Messaging.Consumers.Command
{
    public class TicketReservationConsumer : IConsumer<TicketReservationCommandResponse>
    {
        private readonly ILogger<TicketReservationConsumer> _logger;
        private readonly MessageDispatcher _messageDispatcher;

        public TicketReservationConsumer(
            ILogger<TicketReservationConsumer> logger,
            MessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Consume(ConsumeContext<TicketReservationCommandResponse> context)
        {
            var response = context.Message;
            var connectionId = response.ConnectionId;
            _logger.LogInformation($"Received TicketReservationCommandResponse for ConnectionId: {connectionId}");
            var message = new TicketReservationMessage
            {
                ConnectionId = connectionId,
                MessageItems = new List<TicketReservationDto>
                {
                    new TicketReservationDto
                    {
                        TicketId = response.TicketId
                    }
                }
            };
            await _messageDispatcher.Dispatch("ReceiveTicketReservationCommandResponse", message);
            await Task.CompletedTask;
        }
    }
}
