using Contracts.Query.Abstraction;

namespace Contracts.Query
{
    public record GetStationsQuery : IQuery;
    public record GetAvailableSeatsQuery : IQuery
    {
        public Guid TrainCompositionId { get; init; }
        public Guid StartStation { get; init; }
        public Guid EndStation { get; init; }
    }
    public record GetTrainConnectionsQuery : IQuery
    {
        public Guid StartStation { get; init; }
        public Guid EndStation { get; init; }
        public DateTime DepartureTime { get; init; }
    }
}
