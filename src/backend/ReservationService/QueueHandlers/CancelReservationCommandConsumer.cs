using Contracts.Messages.Backend.Command;
using MassTransit;

namespace ReservationsService.QueueHandler
{
    public class CancelReservationCommandConsumer : IConsumer<CancelReservationCommand>
    {
        private readonly ILogger<ReservationCommandConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;
        private readonly ReservationService.Services.ReservationService _reservationService;

        public CancelReservationCommandConsumer(
            ILogger<ReservationCommandConsumer> logger,
            IPublishEndpoint endpoint,
            ReservationService.Services.ReservationService reservationService)
        {
            _logger = logger;
            _endpoint = endpoint;
            _reservationService = reservationService;
        }

        public async Task Consume(ConsumeContext<CancelReservationCommand> context)
        {
            var message = context.Message;
            var connsectionId = message.ConnectionId;
            var ticketId = message.TicketId;
            _logger.LogInformation($"Received CancelReservationCommand: ReservationId = {message.TicketId}");

            var canceledTicketId = await _reservationService.CancelReservationAsync(ticketId);

            await _endpoint.Publish(new CancelTicketReservationCommandResponse
            {
                ConnectionId = connsectionId,
                TicketId = canceledTicketId
            });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
