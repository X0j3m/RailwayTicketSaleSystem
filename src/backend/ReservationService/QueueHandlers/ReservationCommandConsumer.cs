using Contracts.Messages.Backend.Command;
using MassTransit;

namespace ReservationsService.QueueHandler
{
    public class ReservationCommandConsumer : IConsumer<ReservationCommand>
    {
        private readonly ILogger<ReservationCommandConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;
        private readonly ReservationService.Services.ReservationService _reservationService;

        public ReservationCommandConsumer(
            ILogger<ReservationCommandConsumer> logger,
            IPublishEndpoint endpoint,
            ReservationService.Services.ReservationService reservationService)
        {
            _logger = logger;
            _endpoint = endpoint;
            _reservationService = reservationService;
        }

        public async Task Consume(ConsumeContext<ReservationCommand> context)
        {
            var message = context.Message;

            var connsectionId = message.ConnectionId;
            var seatReservations = message.SeatReservations;
            _logger.LogInformation($"Received ReservationCommand from ConnectionId={message.ConnectionId}: Number of seat reservations={message.SeatReservations.Length}");

            await _reservationService.CreateReservationAsync(seatReservations);

            await _endpoint.Publish(new CommandResponse());
            _logger.LogInformation($"Published response for ConnectionId={message.ConnectionId} for command: {message.GetType().Name}");
        }
    }
}
