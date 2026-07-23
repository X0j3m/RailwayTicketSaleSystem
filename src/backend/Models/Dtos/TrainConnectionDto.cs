using Models.Abstraction;

namespace Models.Dto
{
    public record TrainConnectionDto : IDto
    {
        public List<TrainConnectionSegment> Segments { get; set; } = new List<TrainConnectionSegment>();
        public int TrainChanges { get; init; } = 0;
    }

    public record TrainConnectionSegment : IDto
    {
        private Guid _trainCompositionId;
        private Guid _startStationId;
        private Guid _endStationId;

        public required Guid TrainCompositionId
        {
            get => _trainCompositionId;
            init => _trainCompositionId = value != Guid.Empty
                ? value
                : throw new ArgumentException(nameof(value));
        }
        public required Guid StartStation
        {
            get => _startStationId;
            init => _startStationId = value != Guid.Empty
                ? value
                : throw new ArgumentException(nameof(value));
        }
        public required Guid EndStation
        {
            get => _endStationId;
            init => _endStationId = value != Guid.Empty
                ? value
                : throw new ArgumentException(nameof(value));
        }
        public required DateTimeOffset DepartureTime { get; init; }
        public required DateTimeOffset ArrivalTime { get; init; }
        public required TimeSpan Duration { get; init; } = TimeSpan.Zero;
    }
}