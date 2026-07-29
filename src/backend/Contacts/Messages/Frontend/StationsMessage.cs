using Models.Dto;

namespace Contracts.Messages.Frontend
{
    public record StationsMessage : IMessage<TrainStationDto>
    {
        public required string ConnectionId { get; init; }
        public required IReadOnlyCollection<TrainStationDto> MessageItems { get; init; }
    }
}