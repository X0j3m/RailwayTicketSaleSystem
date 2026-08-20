using Contracts.Messages.Backend.Command;
using Contracts.Messages.Frontend;
using MassTransit;
using Models.Dtos;
using WebAPI.Hubs.Utils;

namespace WebAPI.Messaging.Consumers.Command
{
    public class CancelTicketReservationConsumer : IConsumer<CancelTicketReservationCommandResponse>
    {
        private readonly ILogger<CancelTicketReservationConsumer> _logger;
        private readonly MessageDispatcher _messageDispatcher;

        public CancelTicketReservationConsumer(
            ILogger<CancelTicketReservationConsumer> logger,
            MessageDispatcher messageDispatcher)
        {
            _logger = logger;
            _messageDispatcher = messageDispatcher;
        }

        public async Task Consume(ConsumeContext<CancelTicketReservationCommandResponse> context)
        {
            var response = context.Message;
            var connectionId = response.ConnectionId;
            _logger.LogInformation($"Received CancelTicketReservationCommandResponse for ConnectionId: {connectionId}");
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
            await _messageDispatcher.Dispatch("ReceiveCancelTicketReservationCommandResponse", message);
            await Task.CompletedTask;
        }
    }
}
