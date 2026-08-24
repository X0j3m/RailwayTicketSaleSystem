using Contracts.Messages.Backend.Query;
using Dapper;
using MassTransit;
using Microsoft.Data.SqlClient;
using Models.Dto;
using Models.Dtos;
using System.Data;

namespace TrainFleetService.Service
{
    public class FleetService
    {
        private readonly ILogger<FleetService> _logger;
        private readonly string _connectionString;

        public FleetService(
            ILogger<FleetService> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MicrosoftSQLServer")
                ?? throw new ArgumentNullException("MicrosoftSQLServer");
        }

        public async Task<List<TrainStationDto>> GetStationsAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var getTrainStationsCmd = new CommandDefinition(
                    commandText: TRAIN_STATIONS_QUERY_STRING,
                    commandTimeout: 5,
                    cancellationToken: cancellationToken
                );

                var trainStations = await connection.QueryAsync<TrainStationDto>(getTrainStationsCmd);
                var result = trainStations.ToList();

                _logger.LogInformation("Fetched {Count} trainStations", result.Count);
                return result;
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                _logger.LogWarning(ex, "SQL Timeout while fetching train stations");
                throw new TimeoutException($"Database query timed out", ex);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Fetching train stations was canceled by MassTransit timeout.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching train stations");
                throw;
            }
        }

        public async Task<TrainCompositionDto> GetTrainCompositionAsync(
            GetTrainCompositionAvailableSeatsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var getSeatsInfoCmd = new CommandDefinition(
                    commandText: SEATS_INFO_QUERY_STRING,
                    parameters: new { trainCompositionId = query.TrainCompositionId },
                    commandTimeout: 10,
                    cancellationToken: cancellationToken
                );

                var getTrainInfoCmd = new CommandDefinition(
                    commandText: TRAIN_INFO_QUERY_STRING,
                    parameters: new { trainCompositionId = query.TrainCompositionId },
                    commandTimeout: 10,
                    cancellationToken: cancellationToken
                );

                var getOccupiedSeatsCmd = new CommandDefinition(
                    commandText: OCCUPIED_SEATS_QUERY_STRING,
                    parameters: new
                    {
                        trainCompositionId = query.TrainCompositionId,
                        departureTime = query.DepartureTime,
                        arrivalTime = query.ArrivalTime
                    },
                    commandTimeout: 10,
                    cancellationToken: cancellationToken
                );

                var trainSeats = await connection.QueryAsync<TrainCompositionSeatInfoDto>(getSeatsInfoCmd);
                var trainInfoIEnumerable = await connection.QueryAsync<TrainInfoDto>(getTrainInfoCmd);
                var occupiedSeats = await connection.QueryAsync<OccupiedSeatDto>(getOccupiedSeatsCmd);

                _logger.LogInformation($"Train composition {query.TrainCompositionId} found, {trainSeats.Count()} seats total, {occupiedSeats.Count()} seats occupied.");

                if (trainInfoIEnumerable == null && trainInfoIEnumerable.ToArray().Length > 0)
                {
                    throw new Exception("Train info not found");
                }
                var trainInfo = trainInfoIEnumerable.ToArray()[0];

                var cars = new List<CarDto>();
                foreach (var trainSeat in trainSeats)
                {
                    if (trainSeat == null)
                    {
                        throw new Exception("Train seat not found");
                    }

                    while (cars.Count < trainSeat.CarNumber)
                    {
                        cars.Add(new CarDto
                        {
                            Number = cars.Count + 1
                        });
                    }

                    cars[trainSeat.CarNumber - 1].Seats.Add(new SeatDto
                    {
                        Number = trainSeat.SeatNumber,
                        XPosition = trainSeat.SeatXPos,
                        YPosition = trainSeat.SeatYPos,
                        Occupied = occupiedSeats.Any(s =>
                            s.CarNumber == trainSeat.CarNumber &&
                            s.SeatNumber == trainSeat.SeatNumber)
                    });
                }

                return new TrainCompositionDto
                {
                    TrainCompositionId = query.TrainCompositionId,
                    StartStationId = query.StartStation,
                    EndStationId = query.EndStation,
                    TrainType = trainInfo.TrainType,
                    TrainNumber = trainInfo.TrainNumber,
                    Cars = cars
                };
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                _logger.LogWarning(ex, "SQL Timeout while fetching seats");
                throw new TimeoutException($"Database query timed out", ex);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Fetching seats was canceled by MassTransit timeout.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching seats");
                throw;
            }
        }


        private const string TRAIN_STATIONS_QUERY_STRING =
            @"
                SELECT
                    [id] AS Id,
                    [city] AS City,
                    [name] AS Name,
                    [latitude] AS Latitude,
                    [longitude] AS Longitude
               FROM [TrainStations];
            ";

        private const string OCCUPIED_SEATS_QUERY_STRING =
            @"
                SET DATEFORMAT ymd;
                SELECT 
                    [car_number] AS CarNumber,
                    [seat_number] AS SeatNumber
                FROM 
                    [TicketSegments]
                JOIN
                    [Tickets]
                    ON
                    [Tickets].[id]=[TicketSegments].[ticket_id]
                WHERE 
                    [TicketSegments].[train_composition_id] = '79C90466-BED2-4DE3-B2A6-45BD6BB8A6AC'
                    AND
                    [TicketSegments].[departure_time] < '2026-08-24 17:05:00.000'
                    AND
                    [TicketSegments].[arrival_time] > '2026-08-24 16:30:00.000'
                    AND
                    [Tickets].[status] != 'CANCELLED'
                ORDER BY car_number, seat_number;
            ";

        private const string SEATS_INFO_QUERY_STRING =
            @"
                SELECT
                    TrainCompositions_Cars.[car_number] AS CarNumber,
                    Seats.[number] AS SeatNumber,
                    Seats.[x_pos] AS SeatXPos,
                    Seats.[y_pos] AS SeatYPos
                FROM TrainCompositions
                JOIN TrainCompositions_Cars
                    ON TrainCompositions.id = TrainCompositions_Cars.composition_id
                JOIN Seats
                    ON Seats.car_id = TrainCompositions_Cars.car_id
                WHERE TrainCompositions.id = @trainCompositionId
                ORDER BY TrainCompositions_Cars.car_number, Seats.number;
            ";

        private const string TRAIN_INFO_QUERY_STRING =
            @"
                SELECT Trains.[type] AS TrainType,
                       Trains.[number] AS TrainNumber
                FROM Trains
                JOIN TrainCompositions
                  ON TrainCompositions.train_id = Trains.id
                WHERE TrainCompositions.id = @trainCompositionId;
            ";
    }
}