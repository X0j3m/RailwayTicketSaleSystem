using Models.Dtos;

namespace Contracts.Messages.Frontend
{
    public record TicketsMessage : IMessage<TicketDto>
    {
        public required string ConnectionId { get; init; }
        public required IReadOnlyCollection<TicketDto> MessageItems { get; init; }
    }
}
