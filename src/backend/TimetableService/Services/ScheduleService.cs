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
            string departureTime,
            int departureWindowMinutes,
            int maxNumOfTransfers)
        {
            var query = GetTrainConnectionsQueryString();

            if (!TimeSpan.TryParse(departureTime, out var depTimeSpan))
            {
                depTimeSpan = TimeSpan.Zero;
            }
            int departureTimeMinutes = (int)depTimeSpan.TotalMinutes;

            var queryParameters = new
            {
                sourceStationId,
                targetStationId,
                departureTimeMinutes,
                departureWindowMinutes,
                maxNumOfTransfers
            };

            await using var session = _driver.AsyncSession();

            var result = new List<TrainConnectionDto>();
            try
            {
                var resultCursor = await session.RunAsync(query, queryParameters);

                while (await resultCursor.FetchAsync())
                {
                    var record = resultCursor.Current;

                    var departure = record["departure"].As<string>();
                    var arrival = record["arrival"].As<string>();
                    var totalTripTime = record["total_trip_time"].As<int>();
                    var numOfTransfers = record["num_of_transfers"].As<int>();
                    var relationTypes = record["relation_types"].As<List<string>>();
                    var stationIds = record["station_ids"].As<List<string>>();
                    var rawTransferDetails = record["transfer_details"].As<List<object>>();

                    List<TransferDetail> transferDetails = rawTransferDetails
                        .OfType<IReadOnlyDictionary<string, object>>()
                        .Select(dict => new TransferDetail
                        {
                            ArrivalTime = dict["arrival_time"].As<string>(),
                            DepartureTime = dict["departure_time"].As<string>(),
                            StationId = dict["station_id"].As<string>(),
                            TransferTime = dict["transfer_time"].As<int>(),
                        })
                        .ToList();

                    var trainConnectionDto = new TrainConnectionDto
                    {
                        DepartureTime = departure,
                        ArrivalTime = arrival,
                        TotalTripTime = totalTripTime,
                        RelationTypes = relationTypes,
                        TransferDetails = transferDetails,
                        NumOfTransfers = numOfTransfers,
                        StationIds = stationIds
                    };

                    result.Add(trainConnectionDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

            return result;
        }

        private string GetTrainConnectionsQueryString()
        {
            return @"
        WITH 
          toLower($sourceStationId) AS sourceStationId,
          toLower($targetStationId) AS targetStationId,
          $departureTimeMinutes AS minDepartureTime,
          $departureWindowMinutes AS departureWindowMinutes,
          $maxNumOfTransfers AS maxTransfers

        MATCH (sourceStation:TrainStation {station_id: sourceStationId})<-[:LOCATED_AT]-(startStop:Stop)
        WHERE startStop.departure_time_minutes >= minDepartureTime
          AND startStop.departure_time_minutes <= minDepartureTime + departureWindowMinutes

        MATCH (targetStation:TrainStation {station_id: targetStationId})<-[:LOCATED_AT]-(endStop:Stop)

        CALL apoc.algo.dijkstra(
          startStop,
          endStop,
          'LEADS_TO>|TRANSFER>',
          'time'
        ) YIELD path, weight

        WITH path, startStop, endStop, weight, targetStationId,
             relationships(path) AS rels,
             nodes(path) AS sequenceNodes,
             size([r IN relationships(path) WHERE type(r) = 'TRANSFER']) AS num_of_transfers,
             [n IN nodes(path) | n.station_id] AS stationIds

        WHERE 
          num_of_transfers <= maxTransfers
          
          AND type(rels[0]) = 'LEADS_TO'
          AND type(rels[-1]) = 'LEADS_TO'
          
          AND NONE(i IN range(0, size(rels) - 2) WHERE
            type(rels[i]) = 'TRANSFER' AND type(rels[i+1]) = 'TRANSFER'
          )
          
          AND NONE(i IN range(2, size(stationIds) - 1) WHERE stationIds[i] IN stationIds[..i-1])
          
          AND ALL(sid IN stationIds[..-1] WHERE sid <> targetStationId)

        RETURN
          startStop.departure_time AS departure,
          endStop.arrival_time AS arrival,
          weight AS total_trip_time,
          [r IN rels | type(r)] AS relation_types,
          [i IN range(0, size(rels) - 1) WHERE type(rels[i]) = 'TRANSFER' | {
            station_id: sequenceNodes[i].station_id,
            transfer_time: rels[i].time,
            arrival_time: sequenceNodes[i].arrival_time,
            departure_time: sequenceNodes[i+1].departure_time
          }] AS transfer_details,
          num_of_transfers,
          stationIds AS station_ids
        ORDER BY departure ASC, total_trip_time ASC, num_of_transfers ASC";
        }
    }
}
