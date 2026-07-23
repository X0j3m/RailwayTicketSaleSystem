using MassTransit.Internals.GraphValidation;
using MassTransit.SqlTransport.Topology;
using Models.Dto;
using Models.Dtos;
using Neo4j.Driver;
using System.Globalization;
namespace TimetableService.Services
{
    public class ScheduleService
    {
        private readonly ILogger<ScheduleService> _logger;
        private readonly IDriver _driver;

        public ScheduleService(ILogger<ScheduleService> logger, IDriver driver)
        {
            _logger = logger;
            _driver = driver;
        }

        public async Task<List<TrainConnectionDto>> GetTrainConnections(
            string sourceStationId,
            string targetStationId,
            string departureDate,
            string departureTime)
        {
            var query = @"
            MATCH (source:Stop {station_id: toLower($sourceStationId)})
            MATCH (target:Stop {station_id: toLower($targetStationId)})
            WHERE source.departure_time >= $departureTime
            CALL apoc.algo.dijkstra(source, target, 'LEADS_TO>|TRANSFER>', 'time') 
            YIELD path, weight
            WHERE type(relationships(path)[0]) = 'LEADS_TO'
              AND type(relationships(path)[-1]) = 'LEADS_TO'
              AND weight < 1440.0

            WITH path, weight, [r IN relationships(path) WHERE type(r) = 'TRANSFER'] AS transfers

            WITH weight, nodes(path)[0] AS source_node,
                 [nodes(path)[0]] + 
                 reduce(acc = [], r IN transfers | acc + [startNode(r), endNode(r)]) + 
                 [nodes(path)[-1]] AS connection

            RETURN connection, weight AS trip_time
            ORDER BY source_node.departure_time ASC";
            var queryParameters = new { sourceStationId, targetStationId, departureTime };

            await using var session = _driver.AsyncSession();

            var result = new List<TrainConnectionDto>();

            try
            {
                var resultCursor = await session.RunAsync(query, queryParameters);

                while (await resultCursor.FetchAsync())
                {
                    var record = resultCursor.Current;

                    var tripTime = record["trip_time"].As<double>().As<int>();
                    var connectionNodes = record["connection"].As<IReadOnlyList<INode>>();

                    var connectionDto = new TrainConnectionDto { TrainChanges = (connectionNodes.Count / 2) - 1 };

                    for (int i = 0; i < connectionNodes.Count; i += 2)
                    {
                        var startNode = connectionNodes[i];
                        var endNode = connectionNodes[i + 1];

                        var segment = MapNodesPairToSegment(startNode, endNode, departureDate);
                        connectionDto.Segments.Add(segment);
                    }

                    result.Add(connectionDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

            return result;
        }

        private TrainConnectionSegment MapNodesPairToSegment(INode startNode, INode endNode, string departureDate)
        {
            DateTimeOffset date = DateTimeOffset.ParseExact(
                departureDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal
            );

            var departureTimeString = endNode.Properties.GetValueOrDefault("departure_time", null).As<string>();
            var arrivalTimeString = endNode.Properties.GetValueOrDefault("arrival_time", null).As<string>();

            TimeSpan.TryParse(departureTimeString, out TimeSpan departureTimeSpan);
            TimeSpan.TryParse(arrivalTimeString, out TimeSpan arrivalTimeSpan);

            var departureTime = date + departureTimeSpan;
            var arrivalTime = date + arrivalTimeSpan;

            return new TrainConnectionSegment
            {
                TrainCompositionId = Guid.Parse(startNode.Properties.GetValueOrDefault("train_composition_id", Guid.Empty).As<string>()),
                StartStation = Guid.Parse(startNode.Properties.GetValueOrDefault("station_id", Guid.Empty).As<string>()),
                EndStation = Guid.Parse(endNode.Properties.GetValueOrDefault("station_id", Guid.Empty).As<string>()),
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Duration = arrivalTime - departureTime
            };
        }

        public async Task<List<TrainTripDto>> GetDijkstraRouteAsync(string sourceStationId, string targetStationId, string departureTime)
        {
            var query = @"
            MATCH (source:Stop {station_id: toLower($sourceStationId)})
            MATCH (target:Stop {station_id: toLower($targetStationId)})
            WHERE source.departure_time < $departureTime
            CALL apoc.algo.dijkstra(source, target, 'LEADS_TO>|TRANSFER>', 'time') 
            YIELD path, weight
            WHERE type(relationships(path)[0]) = 'LEADS_TO'
              AND type(relationships(path)[-1]) = 'LEADS_TO'
            RETURN path, weight AS trip_time";
            var queryParameters = new { sourceStationId, targetStationId, departureTime };

            await using var session = _driver.AsyncSession();

            var results = new List<TrainTripDto>();
            try
            {
                var resultCursor = await session.RunAsync(query, queryParameters);

                while (await resultCursor.FetchAsync())
                {
                    var record = resultCursor.Current;

                    var tripTime = record["trip_time"].As<double>().As<int>();

                    var path = record["path"].As<IPath>();
                    var tripResult = new TrainTripDto { TripTime = tripTime };

                    bool isFirst = true;
                    var nodes = path.Nodes;
                    var relationships = path.Relationships;

                    if (nodes.Count > 0)
                    {
                        tripResult.Path.Add(MapNode(nodes[0]));
                    }

                    for (int i = 0; i < relationships.Count; i++)
                    {
                        tripResult.Path.Add(MapRelationship(relationships[i]));
                        tripResult.Path.Add(MapNode(nodes[i + 1]));
                    }

                    results.Add(tripResult);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

            return results;
        }

        private StopDto MapNode(INode node)
        {
            return new StopDto
            {
                StopId = node.Properties.GetValueOrDefault("stop_id", null).As<string>(),
                StationId = node.Properties.GetValueOrDefault("station_id", null).As<string>(),
                ArrivalTime = node.Properties.GetValueOrDefault("arrival_time", null).As<string>(),
                DepartureTime = node.Properties.GetValueOrDefault("departure_time", null).As<string>(),
                ArrivalTimeMinutes = node.Properties.ContainsKey("arrival_time_minutes") ? node.Properties["arrival_time_minutes"].As<int>() : null,
                DepartureTimeMinutes = node.Properties.ContainsKey("departure_time_minutes") ? node.Properties["departure_time_minutes"].As<int>() : null,
                StartStationTime = node.Properties.GetValueOrDefault("start_station_time", null).As<string>(),
                TrainCompositionId = node.Properties.GetValueOrDefault("train_composition_id", null).As<string>()
            };

        }

        private TrainTripSegment MapRelationship(IRelationship rel)
        {
            return new TrainTripSegment
            {
                Type = rel.Type,
                Time = rel.Properties.GetValueOrDefault("time", 0.0).As<double>().As<int>()
            };
        }
    }
}
