namespace Models.Dto
{
    public record TrainStationDto
    {
        public Guid id { get; init; }
        public string city { get; init; }
        public string name { get; init; }
        public double latitude { get; init; }
        public double longitude { get; init; }
    }
}
