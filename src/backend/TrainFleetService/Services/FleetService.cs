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
    }
}
