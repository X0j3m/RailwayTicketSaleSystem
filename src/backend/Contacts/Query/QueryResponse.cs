namespace Contracts.Query
{
    public record StationsQueryResponse : IQueryResponse {
        public List<string> Stations { get; init; } = new();
    }
}
