using Contracts.Query.Abstraction;
using Models.Dto;

namespace Contracts.Query
{
    public record StationsQueryResponse : IQueryResponse
    {
        public List<TrainStationDto> Stations { get; init; } = new();
    }

    public record TrainConnectionsQueryResponse : IQueryResponse
    {
        public List<TrainConnectionDto> Connections { get; init; } = new();
    }

    public record AvailableSeatsQueryResponse : IQueryResponse
    {
        public TrainCompositionDto Train { get; init; } = new();
    }
}