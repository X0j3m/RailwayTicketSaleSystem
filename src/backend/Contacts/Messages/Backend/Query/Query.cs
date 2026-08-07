using Contracts.Messages.Backend.Query.Abstraction;

namespace Contracts.Messages.Backend.Query
{
    public record GetStationsQuery : IQuery
    {
        public required string ConnectionId { get; init; }
    }

    public record GetAvailableSeatsQuery : IQuery
    {
        public required string ConnectionId { get; init; }
        public required GetTrainCompositionAvailableSeatsQuery[] TrainCompositionAvailableSeatsQueries { get; init; }
    }

    public record GetTrainCompositionAvailableSeatsQuery
    {
        public Guid TrainCompositionId { get; init; }
        public Guid StartStation { get; init; }
        public Guid EndStation { get; init; }
        public required DateTime DepartureTime { get; init; }
        public required DateTime ArrivalTime { get; init; }
    }

    public record GetTrainConnectionsQuery : IQuery
    {
        public required string ConnectionId { get; init; }
        public required string StartStation { get; init; }
        public required string EndStation { get; init; }
        public required string DepartureDate { get; init; }
        public required string DepartureTime { get; init; }
        public required int PageNumber { get; init; }
        public required int PageSize { get; init; }
    }
}
