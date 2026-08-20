using Contracts.Messages.Backend.Command.Abstraction;

namespace Contracts.Messages.Backend.Command
{
    public record TicketReservationCommandResponse : ICommandResponse
    {
        public required string ConnectionId { get; init; }
        public required Guid TicketId { get; init; }
    }
    public record CancelTicketReservationCommandResponse : ICommandResponse
    {
        public required string ConnectionId { get; init; }
        public required Guid TicketId { get; init; }
    }
}
