using Models.Abstraction;

namespace Contracts.Messages.Frontend
{
    public interface IMessage
    {
        string ConnectionId { get; init; }
    }

    public interface IMessage<out T> : IMessage where T : IDto
    {
        IReadOnlyCollection<T> MessageItems { get; }
    }
}