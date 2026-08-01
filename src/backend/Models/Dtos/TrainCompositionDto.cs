using Models.Abstraction;

namespace Models.Dto
{
    public record TrainCompositionDto : IDto
    {
        public Guid TrainCompositionId { get; init; }
        public Guid StartStationId { get; init; }
        public Guid EndStationId { get; init; }
        public string TrainType { get; init; } = string.Empty;
        public int TrainNumber { get; init; }
        public List<CarDto> Cars { get; init; } = new List<CarDto>();
    }
    public record TrainCompositionSeatInfoDto : IDto
    {
        public required int CarNumber { get; init; }
        public required int SeatNumber { get; init; }
        public required int SeatXPos { get; init; }
        public required int SeatYPos { get; init; }
    }

    public record TrainInfoDto : IDto
    {
        public required string TrainType { get; init; }
        public required int TrainNumber { get; init; }
    }
}
