using Contracts.Messages.Backend.Query;
using MassTransit;
using Models.Dto;
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
            var trainCompositionQueries = message.TrainCompositionAvailableSeatsQueries;

            _logger.LogInformation($"Received GetAvailableSeatsQuery: ConnectionId={connectionId}, Number of trains={trainCompositionQueries.Length}");

            var response = new List<TrainCompositionDto>();

            foreach (var query in trainCompositionQueries)
            {
                var trainCompositionId = query.TrainCompositionId;
                var startStation = query.StartStation;
                var endStation = query.EndStation;
                var departureTime = query.DepartureTime;
                var arrivalTime = query.ArrivalTime;

                _logger.LogInformation($"Processing query for ConnectionId={connectionId}: TrainCompositionId={trainCompositionId}, StartStation={startStation}, EndStation={endStation}, DepartureTime={departureTime}, ArrivalTime={arrivalTime}");

                var result = await _fleetService.GetTrainCompositionAsync(query);

                response.Add(result);
            }

            await _endpoint.Publish(new AvailableSeatsQueryResponse
            {
                ConnectionId = connectionId,
                Trains = response.ToArray()
            });

            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
