using Contracts;
using Contracts.Messages.Backend.Command;
using Contracts.Messages.Backend.Query;
using Contracts.Messages.Backend.Query.Abstraction;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Messaging.Senders;

namespace WebAPI.Hubs
{
    public class MessageHub : Hub
    {
        private readonly ILogger<MessageHub> _logger;
        private readonly QuerySender _querySender;
        private readonly CommandSender _commandSender;

        public MessageHub(
            ILogger<MessageHub> logger,
            QuerySender querySender,
            CommandSender commandSender)
        {
            _logger = logger;
            _querySender = querySender;
            _commandSender = commandSender;
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

        public async Task GetTickets(GetTicketsQuery query)
        {
            var connectionId = query.ConnectionId;
            _logger.LogInformation($"Sending GetTicketsQuery from connectionId={connectionId}");
            await _querySender.SendQueryAsync(query, QueueNames.ReservationServiceQueue);
        }

        public async Task CreateReservation(ReservationCommand command)
        {
            var connectionId = command.ConnectionId;
            _logger.LogInformation($"Sending CreateReservationCommand from connectionId={connectionId}");
            await _commandSender.SendCommandAsync(command, QueueNames.ReservationServiceQueue);
        }

        public async Task CancelReservation(CancelReservationCommand command)
        {
            var connectionId = command.ConnectionId;
            _logger.LogInformation($"Sending CancelReservationCommand from connectionId={connectionId}");
            await _commandSender.SendCommandAsync(command, QueueNames.ReservationServiceQueue);
        }
    }
}
