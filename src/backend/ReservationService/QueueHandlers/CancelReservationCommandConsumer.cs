using Contracts.Messages.Backend.Command;
using MassTransit;

namespace ReservationsService.QueueHandler
{
    public class CancelReservationCommandConsumer : IConsumer<CancelReservationCommand>
    {
        private readonly ILogger<ReservationCommandConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;

        public CancelReservationCommandConsumer(ILogger<ReservationCommandConsumer> logger, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<CancelReservationCommand> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Received CancelReservationCommand: ReservationId = {message.ReservationId}");

            await Task.Delay(3000);

            await _endpoint.Publish(new CommandResponse());
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
