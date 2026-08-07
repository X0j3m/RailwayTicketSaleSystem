namespace ReservationService.Model
{
    public record TicketSegmentSqlEntity
    {
        public required Guid Id { get; init; }
        public required Guid TicketId { get; init; }
        public required int SegmentNumber { get; init; }
        public required Guid TrainCompositionId { get; init; }
        public required int CarNumber { get; init; }
        public required int SeatNumber { get; init; }
        public required DateTime DepartureTime { get; init; }
        public required DateTime ArrivalTime { get; init; }
        public required Guid StartStationId { get; init; }
        public required Guid EndStationId { get; init; }
    }
}