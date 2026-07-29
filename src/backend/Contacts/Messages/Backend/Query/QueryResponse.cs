using Contracts.Messages.Backend.Query.Abstraction;
using Models.Dto;

namespace Contracts.Messages.Backend.Query
{
    public record StationsQueryResponse : IQueryResponse
    {
        public required string ConnectionId { get; init; }
        public List<TrainStationDto> Stations { get; init; } = new();
    }

    public record TrainConnectionsQueryResponse : IQueryResponse
    {
        public required string ConnectionId { get; init; }
        public List<TrainConnectionDto> Connections { get; init; } = new();
    }

    public record AvailableSeatsQueryResponse : IQueryResponse
    {
        public required string ConnectionId { get; init; }
        public TrainCompositionDto Train { get; init; } = new();
    }
}