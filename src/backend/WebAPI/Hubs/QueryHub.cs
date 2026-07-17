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

        public async Task GetAllStations()
        {
            var query = new GetStationsQuery();

            _logger.LogInformation("Sending GetStationsQuery");
            await _querySender.SendQueryAsync(query, QueueNames.TrainFleetServiceQueue);
        }

        public async Task GetTrainCompositions(GetAvailableSeatsQuery query)
        {
            _logger.LogInformation("Sending GetAvailableSeatsQuery");
            await _querySender.SendQueryAsync(query, QueueNames.TrainFleetServiceQueue);
        }

        public async Task GetTrainConnections(GetTrainConnectionsQuery query)
        {
            _logger.LogInformation("Sending GetTrainConnectionsQuery");
            await _querySender.SendQueryAsync(query, QueueNames.TimetableServiceQueue);
        }
    }
}
