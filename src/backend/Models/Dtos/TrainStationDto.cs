using Models.Abstraction;

namespace Models.Dto
{
    public record TrainStationDto : IDto
    {
        public Guid id { get; init; }
        public string city { get; init; } = string.Empty;
        public string name { get; init; } = string.Empty;
        public double latitude { get; init; }
        public double longitude { get; init; }
    }
}
