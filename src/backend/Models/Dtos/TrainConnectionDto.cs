using Models.Abstraction;

namespace Models.Dto
{
    public record TrainConnectionDto : IDto
    {
        public Guid TrainCompositionId { get; init; }
        public Guid StartStation { get; init; }
        public Guid EndStation { get; init; }
        public DateTimeOffset DepartureTime { get; init; }
        public DateTimeOffset ArrivalTime { get; init; }
        public TimeSpan Duration { get; init; } = TimeSpan.Zero;
        public int TrainChanges { get; init; } = 0;
    }
}