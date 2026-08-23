using Contracts.Messages.Backend.Query;
using MassTransit;

namespace ReservationService.QueueHandlers
{
    public class GetTicketsQueryConsumer : IConsumer<GetTicketsQuery>
    {
        private readonly ILogger<GetTicketsQueryConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;
        private readonly ReservationService.Services.ReservationService _reservationService;

        public GetTicketsQueryConsumer(
            ILogger<GetTicketsQueryConsumer> logger,
            IPublishEndpoint endpoint,
            ReservationService.Services.ReservationService reservationService)
        {
            _logger = logger;
            _endpoint = endpoint;
            _reservationService = reservationService;
        }

        public async Task Consume(ConsumeContext<GetTicketsQuery> context)
        {
            var message = context.Message;
            var cancellationToken = context.CancellationToken;

            var connectionId = message.ConnectionId;
            var email = message.Email;

            _logger.LogInformation($"Received GetTicketsQuery: Email = {email}");

            var tickets = await _reservationService.GetTicketsByEmailAsync(email, cancellationToken);

            await _endpoint.Publish(new GetTicketsQueryResponse
            {
                ConnectionId = connectionId,
                Tickets = tickets
            });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
