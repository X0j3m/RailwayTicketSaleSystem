using Contracts.Messages.Backend.Query.Abstraction;

namespace Contracts.Messages.Backend.Query
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
        public string StartStation { get; init; }
        public string EndStation { get; init; }
        public string DepartureDate { get; init; }
        public string DepartureTime { get; init; }
    }
}
