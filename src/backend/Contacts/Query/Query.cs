namespace Contracts.Query
{
    public record GetStationsQuery : IQuery { }
    public record GetAvailableSeatsQuery : IQuery
    {
        public Guid TrainId { get; init; }
    }
    public record GetTrainConnectionsQuery : IQuery
    {
        public Guid StartStation { get; init; }
        public Guid EndStation { get; init; }
    }
}
