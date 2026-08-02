namespace TimetableService.Models
{
    public record TrainSearchCriteria
    {
        public required string SourceStationId { get; init; }
        public required string TargetStationId { get; init; }
        public required string DepartureDate { get; init; }
        public required string DepartureTime { get; init; }
        public required int MaxNumOfTransfers { get; init; }
        public required int PageSize { get; init; }
        public required int PageNumber { get; init; }
    }
}
