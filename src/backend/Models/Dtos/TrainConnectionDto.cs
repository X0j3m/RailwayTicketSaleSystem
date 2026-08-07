using Models.Abstraction;

namespace Models.Dto
{
    public record TrainConnectionDto : IDto
    {
        public string DepartureTime { get; init; }
        public string ArrivalTime { get; init; }
        public int TotalTripTime { get; init; }
        public List<Transit> Transits { get; init; }
        public List<TransferDetail> TransferDetails { get; init; }
        public int NumOfTransfers { get; init; }
        public List<string> StationIds { get; init; }
        public List<string> TrainCompositionIds { get; init; }
    }

    public record TransferDetail
    {
        public required string StationId { get; init; }
        public required string ArrivalTime { get; init; }
        public required string DepartureTime { get; init; }
        public int TransferTime { get; init; }
    }

    public record Transit
    {
        public required string FromStationId { get; init; }
        public required string ToStationId { get; init; }
        public required string ArrivalTime { get; init; }
        public required string DepartureTime { get; init; }
        public required int TravelTime { get; init; }
        public required string TrainCompositionId { get; init; }
    }
}