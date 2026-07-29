using Models.Abstraction;

namespace Models.Dto
{
    public record TrainConnectionDto : IDto
    {
        public string DepartureTime { get; init; }
        public string ArrivalTime { get; init; }
        public int TotalTripTime { get; init; }
        public List<string> RelationTypes { get; init; }
        public List<TransferDetail> TransferDetails { get; init; }
        public int NumOfTransfers { get; init; }
        public List<string> StationIds { get; init; }
        public List<string> TrainCompositionIds { get; init; }
    }

    public record TransferDetail
    {
        public string StationId { get; init; }
        public string ArrivalTime { get; init; }
        public string DepartureTime { get; init; }
        public int TransferTime { get; init; }
    }    
}