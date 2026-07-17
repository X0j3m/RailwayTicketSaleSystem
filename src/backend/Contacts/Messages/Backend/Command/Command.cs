using Contracts.Messages.Backend.Command.Abstraction;

namespace Contracts.Messages.Backend.Command
{
    public record SeatReservation
    {
        public Guid TrainCompositon { get; init; }
        public int CarNumber { get; init; }
        public int SeatNumber { get; init; }
        public Guid FromStationId { get; init; }
        public Guid ToStationId { get; init; }
    }

    public record ReservationCommand(SeatReservation[] SeatReservations) : ICommand { }
    public record CancelReservationCommand(Guid ReservationId) : ICommand { }
}
