namespace Models.Dtos
{
    public record TrainTripDto
    {
        public List<object> Path { get; init; } = new List<object>();
        public required int TripTime { get; init; }
    }

    public record TrainTripSegment
    {
        public required string Type { get; set; }
        public required int Time { get; set; }
    }
}
