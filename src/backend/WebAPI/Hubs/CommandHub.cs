using Contracts;
using Contracts.Messages.Backend.Command;
using Microsoft.AspNetCore.SignalR;
using WebAPI.Messaging.Senders;

namespace WebAPI.Hubs
{
    public class CommandHub : Hub
    {
        private readonly ILogger<CommandHub> _logger;
        private readonly CommandSender _commandSender;

        public CommandHub(ILogger<CommandHub> logger, CommandSender commandSender)
        {
            _logger = logger;
            _commandSender = commandSender;
        }

        public async Task CreateReservation(ReservationCommand command)
        {
            await _commandSender.SendCommandAsync(command, QueueNames.ReservationServiceQueue);
        }

        public async Task CancelReservation(CancelReservationCommand command)
        {
            await _commandSender.SendCommandAsync(command, QueueNames.ReservationServiceQueue);
        }
    }
}
