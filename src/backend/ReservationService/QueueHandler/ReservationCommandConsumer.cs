using Contracts.Messages.Command;
using MassTransit;

namespace ReservationsService.QueueHandler
{
    public class ReservationCommandConsumer : IConsumer<ReservationCommand>
    {
        private readonly ILogger<ReservationCommandConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;

        public ReservationCommandConsumer(ILogger<ReservationCommandConsumer> logger, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<ReservationCommand> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Received ReservationCommand: {message.SeatReservations}");

            await Task.Delay(3000);

            await _endpoint.Publish(new CommandResponse());
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
