using Contracts.Messages.Backend.Query;
using Dapper;
using MassTransit;
using Models.Dto;
using Models.Dtos;
using System.Data;

namespace TrainFleetService.Service
{
    public class FleetService
    {
        private readonly ILogger<FleetService> _logger;
        private readonly IDbConnection _dbConnection;

        public FleetService(
            ILogger<FleetService> logger,
            IDbConnection dbConnection)
        {
            _logger = logger;
            _dbConnection = dbConnection;
        }

        public async Task<List<TrainStationDto>> GetStationsAsync()
        {
            var sql = GetTrainStationsQueryString();
            var stations = await _dbConnection.QueryAsync<TrainStationDto>(sql);
            return stations.ToList();
        }

        public async Task<TrainCompositionDto> GetTrainCompositionAsync(GetTrainCompositionAvailableSeatsQuery query)
        {
            var seatsSql = GetSeatsInfoQueryString();
            var trainSql = GetTrainInfoQueryString();
            var occupiedSeatsSql = GetOccupiedSeatsQueryString();

            var trainSeats = await _dbConnection.QueryAsync<TrainCompositionSeatInfoDto>(
                seatsSql,
                new { trainCompositionId = query.TrainCompositionId });

            var trainInfo = await _dbConnection.QueryFirstOrDefaultAsync<TrainInfoDto>(
                trainSql,
                new { trainCompositionId = query.TrainCompositionId });

            var occupiedSeats = await _dbConnection.QueryAsync<OccupiedSeatDto>(
                occupiedSeatsSql,
                new
                {
                    trainCompositionId = query.TrainCompositionId,
                    departureTime = query.DepartureTime,
                    arrivalTime = query.ArrivalTime
                });

            _logger.LogInformation($"Train composition {query.TrainCompositionId} found, {trainSeats.Count()} seats total, {occupiedSeats.Count()} seats occupied.");

            if (trainInfo == null)
            {
                throw new Exception("Train info not found");
            }

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

        private string GetTrainStationsQueryString()
        {
            return @"
                SELECT
                    [id] AS Id,
                    [city] AS City,
                    [name] AS Name,
                    [latitude] AS Latitude,
                    [longitude] AS Longitude
               FROM [TrainStations];
            ";
        }

        private string GetOccupiedSeatsQueryString()
        {
            return @"
                SET DATEFORMAT ymd;
                SELECT 
                    [car_number] AS CarNumber,
                    [seat_number] AS SeatNumber
                FROM 
                    [TicketSegments]
                WHERE 
                    [train_composition_id] = @trainCompositionId
                    AND
                    [departure_time] < @arrivalTime
                    AND
                    [arrival_time] > @departureTime
                ORDER BY car_number, seat_number;
            ";
        }

        private string GetSeatsInfoQueryString()
        {
            return @"
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
        }

        private string GetTrainInfoQueryString()
        {
            return @"
                SELECT Trains.[type] AS TrainType,
                       Trains.[number] AS TrainNumber
                FROM Trains
                JOIN TrainCompositions
                  ON TrainCompositions.train_id = Trains.id
                WHERE TrainCompositions.id = @trainCompositionId;
            ";
        }
    }
}