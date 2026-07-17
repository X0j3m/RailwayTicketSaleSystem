using Models.Abstraction;

namespace Models.Dto
{
    public record TrainCompositionDto : IDto
    {
        public string TrainType { get; init; } = string.Empty;
        public int TrainNumber { get; init; }
        public List<CarDto> Cars { get; init; } = new List<CarDto>();
    }
}
