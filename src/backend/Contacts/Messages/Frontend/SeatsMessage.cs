using Models.Dto;

namespace Contracts.Messages.Frontend
{
    public record SeatsMessage : IMessage<TrainCompositionDto>
    {
        public required string ConnectionId { get; init; }
        public required IReadOnlyCollection<TrainCompositionDto> MessageItems { get; init; }
    }
}
