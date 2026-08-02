using Dapper;
using Models.Dto;
using System.Data;

namespace TrainFleetService.Service
{
    public class FleetService
    {
        private readonly IDbConnection _dbConnection;

        public FleetService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<List<TrainStationDto>> GetStationsAsync()
        {
            var sql = "SELECT * FROM TrainStations";
            var stations = await _dbConnection.QueryAsync<TrainStationDto>(sql);
            return stations.ToList();
        }

        public async Task<TrainCompositionDto> GetTrainCompositionAsync(Guid trainCompositionId, Guid startStationId, Guid endStationId)
        {
            var trainCompositionIdString = trainCompositionId.ToString();

            var seatsSql = GetSeatsInfoQueryString();
            var trainSql = GetTrainInfoQueryString();
            var trainSeats = await _dbConnection.QueryAsync<TrainCompositionSeatInfoDto>(seatsSql, new { trainCompositionId = trainCompositionIdString });
            var trainInfo = await _dbConnection.QueryFirstOrDefaultAsync<TrainInfoDto>(trainSql, new { trainCompositionId = trainCompositionIdString });

            var cars = new List<CarDto>();
            for (int i = 0; i < trainSeats.Count(); i++)
            {
                var trainSeat = trainSeats.ElementAtOrDefault(i);

                if (cars.Count() < trainSeat.CarNumber)
                {
                    cars.Add(new CarDto()
                    {
                        Number = trainSeat.CarNumber
                    });
                }

                cars[trainSeat.CarNumber - 1].Seats.Add(new SeatDto
                {
                    Number = trainSeat.SeatNumber,
                    XPosition = trainSeat.SeatXPos,
                    YPosition = trainSeat.SeatYPos,
                    Occupied = Random.Shared.NextDouble() > 0.5 ? true : false
                });
            }

            var trainCompositionDto = new TrainCompositionDto
            {
                TrainCompositionId = trainCompositionId,
                StartStationId = startStationId,
                EndStationId = endStationId,
                TrainType = trainInfo.TrainType,
                TrainNumber = trainInfo.TrainNumber,
                Cars = cars
            };

            return trainCompositionDto;
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
                ORDER BY TrainCompositions_Cars.car_number, Seats.number";
        }

        private string GetTrainInfoQueryString()
        {
            return @"
                SELECT Trains.[type] AS TrainType,
                      Trains.[number] AS TrainNumber
                FROM Trains
                JOIN TrainCompositions
                  ON TrainCompositions.train_id = Trains.id

                WHERE TrainCompositions.id = @trainCompositionId";
        }
    }
}
