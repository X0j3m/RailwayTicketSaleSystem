using Models.Dtos;

namespace Contracts.Messages.Frontend
{
    public record TicketReservationMessage : IMessage<TicketReservationDto>
    {
        public required string ConnectionId { get; init; }
        public required IReadOnlyCollection<TicketReservationDto> MessageItems { get; init; }
    }
}
