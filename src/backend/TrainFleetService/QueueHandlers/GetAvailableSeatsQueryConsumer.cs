using Contracts.Messages.Backend.Query;
using MassTransit;
using TrainFleetService.Service;

namespace TrainFleetService.QueueHandler
{
    internal class GetAvailableSeatsQueryConsumer : IConsumer<GetAvailableSeatsQuery>
    {
        private readonly ILogger<GetAvailableSeatsQueryConsumer> _logger;
        private readonly FleetService _fleetService;
        private readonly IPublishEndpoint _endpoint;

        public GetAvailableSeatsQueryConsumer(ILogger<GetAvailableSeatsQueryConsumer> logger, FleetService fleetService, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
            _fleetService = fleetService;
        }

        public async Task Consume(ConsumeContext<GetAvailableSeatsQuery> context)
        {
            var message = context.Message;
            var connectionId = message.ConnectionId;

            var trainCompositionId = message.TrainCompositionId;
            var startStation = message.StartStation;
            var endStation = message.EndStation;
            var departureDate = message.DepartureDate;

            _logger.LogInformation($"Received GetAvailableSeatsQuery: TrainCompositionId={trainCompositionId}, StartStation={startStation}, EndStation={endStation}, DepartureDate={departureDate}");

            var response = await _fleetService.GetTrainCompositionAsync(trainCompositionId, startStation, endStation);

            await _endpoint.Publish(new AvailableSeatsQueryResponse
            {
                ConnectionId = connectionId,
                Train = response
            });

            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
