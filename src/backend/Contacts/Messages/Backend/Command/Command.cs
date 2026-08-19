using Contracts.Messages.Backend.Command.Abstraction;

namespace Contracts.Messages.Backend.Command
{
    public record SeatReservation
    {
        public required Guid TrainComposition { get; init; }
        public required int SegmentNumber { get; init; }
        public required int CarNumber { get; init; }
        public required int SeatNumber { get; init; }
        public required Guid FromStationId { get; init; }
        public required Guid ToStationId { get; init; }
        public required DateTime DepartureTime { get; init; }
        public required DateTime ArrivalTime { get; init; }
    }

    public record ReservationCommand : ICommand
    {
        public required string ConnectionId { get; init; }
        public required string Email { get; init; }
        public required SeatReservation[] SeatReservations { get; init; }
    }

    public record CancelReservationCommand : ICommand
    {
        public required string ConnectionId { get; init; }
        public required Guid TicketId { get; init; }
    }
}
