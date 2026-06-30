namespace Contacts.Query
{
    public record StationListQuery : IQuery { }
    public record TrainSeats(Guid TrainId) : IQuery { }
    public record TrainConnections(Guid StartStation, Guid EndStation) : IQuery { }
}
