using Models.Abstraction;

namespace Models.Dto
{
    public record CarDto : IDto
    {
        public int Number { get; init; }
        public List<SeatDto> Seats { get; init; } = new List<SeatDto>();
    }
}
