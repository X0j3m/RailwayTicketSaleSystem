using MassTransit;
using Contracts.Query;
using Models.Dto;

namespace TrainFleetService.QueueHandler
{
    public class GetStationsQueryConsumer : IConsumer<GetStationsQuery>
    {
        private readonly ILogger<GetStationsQueryConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;

        public GetStationsQueryConsumer(ILogger<GetStationsQueryConsumer> logger, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<GetStationsQuery> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Received query: {message}");

            await Task.Delay(3000);

            await _endpoint.Publish(
                new StationsQueryResponse
                {
                    Stations = new List<TrainStationDto>
                    {
                        new TrainStationDto { id = Guid.NewGuid(), city = "City A", name = "Station A", latitude = 50.0, longitude = 10.0 },
                        new TrainStationDto { id = Guid.NewGuid(), city = "City B", name = "Station B", latitude = 51.0, longitude = 11.0 },
                        new TrainStationDto { id = Guid.NewGuid(), city = "City C", name = "Station C", latitude = 52.0, longitude = 12.0 }
                    }
                });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
