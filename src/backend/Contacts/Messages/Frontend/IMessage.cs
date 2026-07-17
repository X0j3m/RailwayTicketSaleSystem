using Models.Abstraction;

namespace Contracts.Messages.Frontend
{
    public interface IMessage
    {
        string MessageTitle { get; }
    }

    public interface IMessage<out T> : IMessage where T : IDto
    {
        IReadOnlyCollection<T> MessageItems { get; }
    }
}