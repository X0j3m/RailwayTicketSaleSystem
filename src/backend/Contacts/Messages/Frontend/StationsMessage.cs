using Models.Dto;

namespace Contracts.Messages.Frontend
{
    public record StationsMessage : IMessage<TrainStationDto>
    {
        public string MessageTitle { get; } = "Stations";
        public required IReadOnlyCollection<TrainStationDto> MessageItems { get; init; }
    }
}