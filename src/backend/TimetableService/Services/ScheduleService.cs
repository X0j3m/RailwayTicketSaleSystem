using Models.Dto;
using Neo4j.Driver;
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
                    var stationIds = record["station_ids"].As<List<string>>();
                    var trainCompositionIds = record["train_composition_ids"].As<List<string>>();
                    var rawTransferDetails = record["transfer_details"].As<List<object>>();
                    var rawTransits = record["transits"].As<List<object>>();

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

                    List<Transit> transits = rawTransits
                        .OfType<IReadOnlyDictionary<string, object>>()
                        .Select(dict => new Transit
                        {
                            FromStationId = dict["from_station_id"].As<string>(),
                            ToStationId = dict["to_station_id"].As<string>(),
                            ArrivalTime = dict["arrival_time"].As<string>(),
                            DepartureTime = dict["departure_time"].As<string>(),
                            TrainCompositionId = dict["train_composition_id"].As<string>(),
                        })
                        .ToList();

                    var trainConnectionDto = new TrainConnectionDto
                    {
                        DepartureTime = departure,
                        ArrivalTime = arrival,
                        TotalTripTime = totalTripTime,
                        Transits = transits,
                        TransferDetails = transferDetails,
                        NumOfTransfers = numOfTransfers,
                        StationIds = stationIds,
                        TrainCompositionIds = trainCompositionIds
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

            WITH path, startStop, endStop, weight, targetStationId, maxTransfers,
                 relationships(path) AS rels,
                 nodes(path) AS sequenceNodes,
                 size([r IN relationships(path) WHERE type(r) = 'TRANSFER']) AS num_of_transfers,
                 [n IN nodes(path) | n.station_id] AS stationIds,
                 [n IN nodes(path) WHERE n.train_composition_id IS NOT NULL | n.train_composition_id] AS rawIds

            WITH path, startStop, endStop, weight, targetStationId, maxTransfers, rels, sequenceNodes, num_of_transfers, stationIds,
                 [i IN range(0, size(rawIds)-1) WHERE NOT rawIds[i] IN rawIds[..i] | rawIds[i]] AS trainCompositionIds

            WHERE 
              num_of_transfers <= maxTransfers
  
              AND type(rels[0]) = 'LEADS_TO'
              AND type(rels[-1]) = 'LEADS_TO'
  
              AND NONE(i IN range(0, size(rels) - 2) WHERE
                type(rels[i]) = 'TRANSFER' AND type(rels[i+1]) = 'TRANSFER'
              )
  
              AND NONE(i IN range(2, size(stationIds) - 1) WHERE stationIds[i] IN stationIds[..i-1])
  
              AND ALL(sid IN stationIds[..-1] WHERE sid <> targetStationId)

            WITH startStop, endStop, weight, rels, sequenceNodes, num_of_transfers, stationIds, trainCompositionIds,
                 [i IN range(0, size(rels)-1) WHERE i = 0 OR type(rels[i-1]) = 'TRANSFER'] AS segmentStarts,
                 [i IN range(0, size(rels)-1) WHERE i = size(rels)-1 OR type(rels[i+1]) = 'TRANSFER'] AS segmentEnds

            WITH startStop, endStop, weight, rels, sequenceNodes, num_of_transfers, stationIds, trainCompositionIds,
                 [idx IN range(0, size(segmentStarts)-1) | {
                   from_station_id: sequenceNodes[segmentStarts[idx]].station_id,
                   to_station_id: sequenceNodes[segmentEnds[idx] + 1].station_id,
                   train_composition_id: sequenceNodes[segmentStarts[idx]].train_composition_id,
                   departure_time: sequenceNodes[segmentStarts[idx]].departure_time,
                   arrival_time: sequenceNodes[segmentEnds[idx] + 1].arrival_time
                 }] AS transits

            RETURN
              startStop.departure_time AS departure,
              endStop.arrival_time AS arrival,
              weight AS total_trip_time,
              transits,
              [i IN range(0, size(rels) - 1) WHERE type(rels[i]) = 'TRANSFER' | {
                station_id: sequenceNodes[i].station_id,
                transfer_time: rels[i].time,
                arrival_time: sequenceNodes[i].arrival_time,
                departure_time: sequenceNodes[i+1].departure_time
              }] AS transfer_details,
              num_of_transfers,
              stationIds AS station_ids,
              trainCompositionIds AS train_composition_ids
            ORDER BY departure ASC, total_trip_time ASC, num_of_transfers ASC";
        }
    }
}
