using Models.Abstraction;

namespace Models.Dtos
{
    public record TicketReservationDto : IDto
    {
        public required Guid TicketId { get; init; }
    }
}
