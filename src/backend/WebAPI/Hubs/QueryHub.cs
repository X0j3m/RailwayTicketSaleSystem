using Contracts;
using Contracts.Messages.Backend.Query;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Messaging.Senders;

namespace WebAPI.Hubs
{
    public class QueryHub : Hub
    {
        private readonly ILogger<QueryHub> _logger;
        private readonly QuerySender _querySender;

        public QueryHub(ILogger<QueryHub> logger, QuerySender querySender)
        {
            _logger = logger;
            _querySender = querySender;
        }

        public async Task GetAllStations(GetStationsQuery query)
        {
            var connectionId = query.ConnectionId;
            _logger.LogInformation($"Sending GetStationsQuery from connectionId={connectionId}");
            await _querySender.SendQueryAsync(query, QueueNames.TrainFleetServiceQueue);
        }

        public async Task GetTrainCompositions(GetAvailableSeatsQuery query)
        {
            var connectionId = query.ConnectionId;
            _logger.LogInformation($"Sending GetAvailableSeatsQuery from connectionId={connectionId}");
            await _querySender.SendQueryAsync(query, QueueNames.TrainFleetServiceQueue);
        }

        public async Task GetTrainConnections(GetTrainConnectionsQuery query)
        {
            var connectionId = query.ConnectionId;
            _logger.LogInformation($"Sending GetTrainConnectionsQuery from connectionId={connectionId}");
            await _querySender.SendQueryAsync(query, QueueNames.TimetableServiceQueue);
        }
    }
}
