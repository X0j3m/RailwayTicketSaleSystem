namespace Contracts.Messages.Backend.Query.Abstraction
{
    public interface IQuery : IMessage
    {
        public string ConnectionId { get; init; }
    }
}
