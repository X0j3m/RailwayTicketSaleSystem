using Contracts.Messages.Backend.Query;
using MassTransit;
using Models.Dto;
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
            _logger.LogInformation($"Received GetTrainConnectionsQuery: StartStation={message.StartStation}, EndStation={message.EndStation}, DepartureTime={message.DepartureTime}, DepartureDate={message.DepartureDate}");

            var sourceStationId = message.StartStation.ToString();
            var targetStationId = message.EndStation.ToString();
            var departureTime = message.DepartureTime;
            var departureDate = message.DepartureDate;

            var results = await _scheduleService.GetTrainConnections(
                sourceStationId,
                targetStationId,
                departureDate,
                departureTime,
                6 * 60,
                5);

            await _endpoint.Publish(new TrainConnectionsQueryResponse
            {
                Connections = results
            });
            _logger.LogInformation($"Published response for query: {message.GetType().Name}");
        }
    }
}
