using Contracts.Messages.Backend.Query;
using MassTransit;
using TimetableService.Services;

namespace TimetableService.QueueHandler
{
    internal class GetTrainConnectionsQueryConsumer : IConsumer<GetTrainConnectionsQuery>
    {
        private readonly ILogger<GetTrainConnectionsQueryConsumer> _logger;
        private readonly IPublishEndpoint _endpoint;
        private readonly ScheduleService _scheduleService;

        public GetTrainConnectionsQueryConsumer(
            ILogger<GetTrainConnectionsQueryConsumer> logger,
            IPublishEndpoint endpoint,
            ScheduleService scheduleService)
        {
            _logger = logger;
            _endpoint = endpoint;
            _scheduleService = scheduleService;
        }

        public async Task Consume(ConsumeContext<GetTrainConnectionsQuery> context)
        {
            var message = context.Message;
            var connectionId = message.ConnectionId;

            var sourceStationId = message.StartStation.ToString();
            var targetStationId = message.EndStation.ToString();
            var departureTime = message.DepartureTime;
            var departureDate = message.DepartureDate;

            _logger.LogInformation($"Received GetTrainConnectionsQuery: ConnectionId={connectionId}, StartStation={sourceStationId}, EndStation={targetStationId}, DepartureTime={departureTime}, DepartureDate={departureDate}");

            var results = await _scheduleService.GetTrainConnections(
                sourceStationId,
                targetStationId,
                departureDate,
                departureTime,
                6 * 60,
                5);

            await _endpoint.Publish(new TrainConnectionsQueryResponse
            {
                ConnectionId = connectionId,
                Connections = results
            });
            _logger.LogInformation($"Published response for {message.GetType().Name} connectionId={connectionId}");
        }
    }
}
