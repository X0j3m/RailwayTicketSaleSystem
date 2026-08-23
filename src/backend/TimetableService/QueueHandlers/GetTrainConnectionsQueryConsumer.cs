using Contracts.Messages.Backend.Query;
using MassTransit;
using TimetableService.Services;
using TimetableService.Models;

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
            var cancellationToken = context.CancellationToken;
            var connectionId = message.ConnectionId;

            var sourceStationId = message.StartStation.ToString();
            var targetStationId = message.EndStation.ToString();
            var departureTime = message.DepartureTime;
            var departureDate = message.DepartureDate;
            var pageSize = message.PageSize;
            var pageNumber = message.PageNumber;

            _logger.LogInformation($"Received GetTrainConnectionsQuery: ConnectionId={connectionId}, StartStation={sourceStationId}, EndStation={targetStationId}, DepartureTime={departureTime}, DepartureDate={departureDate}");

            var searchCriteria = new TrainSearchCriteria
            {
                SourceStationId = sourceStationId,
                TargetStationId = targetStationId,
                DepartureDate = departureDate,
                DepartureTime = departureTime,
                MaxNumOfTransfers = 5,
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            var resultsPage = await _scheduleService.GetTrainConnections(searchCriteria, cancellationToken);

            await _endpoint.Publish(new TrainConnectionsQueryResponse
            {
                ConnectionId = connectionId,
                ConnectionsPage = resultsPage
            });
            _logger.LogInformation($"Published response for {message.GetType().Name} connectionId={connectionId}");
        }
    }
}
