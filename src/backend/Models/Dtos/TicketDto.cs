using Models.Abstraction;

namespace Models.Dtos
{
    public record TicketDto : IDto
    {
        public required Guid TicketId { get; init; }
        public required string DepartureTime { get; init; }
        public required string ArrivalTime { get; init; }
        public required Guid FromStationId { get; init; }
        public required Guid ToStationId { get; init; }

    }
}
