using Models.Dto;

namespace Contracts.Messages.Frontend
{
    public record TrainConnectionsMessage : IMessage<TrainConnectionDto>
    {
        public string MessageTitle { get; } = "Connections";
        public required IReadOnlyCollection<TrainConnectionDto> MessageItems { get; init; }
    }
}