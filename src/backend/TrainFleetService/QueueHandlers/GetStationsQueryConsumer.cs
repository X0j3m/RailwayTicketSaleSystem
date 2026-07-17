using MassTransit;
using TrainFleetService.Service;
using Contracts.Messages.Backend.Query;

namespace TrainFleetService.QueueHandler
{
    public class GetStationsQueryConsumer : IConsumer<GetStationsQuery>
    {
        private readonly ILogger<GetStationsQueryConsumer> _logger;
        private readonly FleetService _fleetService;
        private readonly IPublishEndpoint _endpoint;

        public GetStationsQueryConsumer(ILogger<GetStationsQueryConsumer> logger, FleetService fleetService, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _fleetService = fleetService;
            _endpoint = endpoint;
        }

        public async Task Consume(ConsumeContext<GetStationsQuery> context)
        {
            var message = context.Message;
            _logger.LogInformation($"Received query: {message}");

            var response = await _fleetService.GetStationsAsync();

            await _endpoint.Publish(new StationsQueryResponse { Stations = response });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
