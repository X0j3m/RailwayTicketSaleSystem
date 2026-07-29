using Models.Dto;

namespace Contracts.Messages.Frontend
{
    public record TrainConnectionsMessage : IMessage<TrainConnectionDto>
    {
        public required string ConnectionId { get; init; }
        public required IReadOnlyCollection<TrainConnectionDto> MessageItems { get; init; }
    }
}