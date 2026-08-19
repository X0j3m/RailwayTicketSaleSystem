using Contracts.Messages.Backend.Query.Abstraction;
using Models.Dto;
using Models.Dtos;

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
        public required TrainConnectionsDtoPage ConnectionsPage { get; init; }
    }

    public record AvailableSeatsQueryResponse : IQueryResponse
    {
        public required string ConnectionId { get; init; }
        public required TrainCompositionDto[] Trains { get; init; }
    }

    public record GetTicketsQueryResponse : IQueryResponse
    {
        public required string ConnectionId { get; init; }
        public required TicketDto[] Tickets { get; init; }
    }
}