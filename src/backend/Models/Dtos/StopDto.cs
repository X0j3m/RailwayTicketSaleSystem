namespace Models.Dtos
{
    public record StopDto
    {
        private string _stopId = default!;
        private string _stationId = default!;
        private string _startStationTime = default!;
        private string _trainCompositionId = default!;

        public required string StopId
        {
            get => _stopId;
            init => _stopId = value ?? throw new ArgumentNullException(nameof(value));
        }
        public required string StationId
        {
            get => _stationId;
            init => _stationId = value ?? throw new ArgumentNullException(nameof(value));
        }
        public string? ArrivalTime { get; init; }
        public string? DepartureTime { get; init; }
        public int? ArrivalTimeMinutes { get; init; }
        public int? DepartureTimeMinutes { get; init; }
        public required string StartStationTime
        {
            get => _startStationTime;
            init => _startStationTime = value ?? throw new ArgumentNullException(nameof(value));
        }
        public required string TrainCompositionId
        {
            get => _trainCompositionId;
            init => _trainCompositionId = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
