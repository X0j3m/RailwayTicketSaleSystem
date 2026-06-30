namespace Contacts.Command
{
    public record SeatReservation
    {
        public Guid TrainCompositon { get; init; }
        public int CarNumber { get; init; }
        public int SeatNumber { get; init; }
        public Guid FromStationId { get; init; }
        public Guid ToStationId { get; init; }
    }

    public record Reservation(SeatReservation[] SeatReservations) : ICommand { }
    public record CancelReservation(Guid ReservationId) : ICommand { }
}
