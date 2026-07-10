namespace Models.Dto
{
    public record CarDto
    {
        public int Number { get; init; }
        public List<SeatDto> Seats { get; init; } = new List<SeatDto>();
    }
}
