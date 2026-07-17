using Models.Abstraction;

namespace Models.Dto
{
    public record SeatDto : IDto
    {
        public int Number { get; init; }
        public int XPosition { get; init; }
        public int YPosition { get; init; }
        public bool Ocupied { get; init; } = false;
    }
}
