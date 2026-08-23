using Contracts.Messages.Backend.Command;
using MassTransit;

namespace ReservationsService.QueueHandler
{
    public class ReservationCommandConsumer : IConsumer<ReservationCommand>
    {
        private readonly ILogger<ReservationCommandConsumer> _logger;
        private readonly ReservationService.Services.ReservationService _reservationService;

        public ReservationCommandConsumer(
            ILogger<ReservationCommandConsumer> logger,
            ReservationService.Services.ReservationService reservationService)
        {
            _logger = logger;
            _reservationService = reservationService;
        }

        public async Task Consume(ConsumeContext<ReservationCommand> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing ReservationCommand for ConnectionId={ConnectionId}", message.ConnectionId);

            var ticketId = await _reservationService.CreateReservationAsync(
                message.Email,
                message.SeatReservations,
                context.CancellationToken);

            await context.Publish(new TicketReservationCommandResponse
            {
                ConnectionId = message.ConnectionId,
                TicketId = ticketId
            }, context.CancellationToken);

            _logger.LogInformation("Published TicketReservationCommandResponse for ConnectionId={ConnectionId}", message.ConnectionId);
        }
    }
}