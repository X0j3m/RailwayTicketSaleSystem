using Contracts.Messages.Backend.Command;
using Dapper;
using MassTransit.Caching.Internals;
using MassTransit.Internals.GraphValidation;
using ReservationService.Model;
using System.Data;
using System.Text;

namespace ReservationService.Services
{
    public class ReservationService
    {
        private readonly ILogger<ReservationService> _logger;
        private readonly IDbConnection _dbConnection;

        public ReservationService(ILogger<ReservationService> logger, IDbConnection dbConnection)
        {
            _logger = logger;
            _dbConnection = dbConnection;
        }

        public async Task<Guid> CreateReservationAsync(SeatReservation[] seatReservations)
        {
            var sb = new StringBuilder();

            sb.AppendLine(@"BEGIN TRANSACTION;
                            SET DATEFORMAT dmy;");

            var ticketId = Guid.NewGuid();

            sb.AppendLine(GetCreateTicketSqlQuery(ticketId));

            foreach (var reservation in seatReservations)
            {
                var ticketSegmentEntity = new TicketSegmentSqlEntity
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticketId,
                    SegmentNumber = reservation.SegmentNumber,
                    TrainCompositionId = reservation.TrainComposition,
                    CarNumber = reservation.CarNumber,
                    SeatNumber = reservation.SeatNumber,
                    DepartureTime = reservation.DepartureTime,
                    ArrivalTime = reservation.ArrivalTime,
                    StartStationId = reservation.FromStationId,
                    EndStationId = reservation.ToStationId,
                };

                sb.AppendLine(GetCreateTicketSegmentSqlQuery(ticketSegmentEntity));
            }

            sb.AppendLine("COMMIT;");

            var sql = sb.ToString();

            _logger.LogInformation($"Executing SQL: \n{sql}");

            await _dbConnection.ExecuteAsync(sql);

            return Guid.NewGuid();
        }

        private string GetCreateTicketSqlQuery(Guid ticketId)
        {
            return $"INSERT INTO [Tickets] ([id]) VALUES('{ticketId}');";
        }

        private string GetCreateTicketSegmentSqlQuery(TicketSegmentSqlEntity ticketSegmentEntity)
        {
            return $@"
                INSERT INTO [TicketSegments] (
                    [id],
                    [ticket_id],
                    [segment_number],
                    [train_composition_id],
                    [car_number],
                    [seat_number],
                    [departure_time],
                    [arrival_time],
                    [start_station_id],
                    [end_station_id])
                VALUES(
	                '{ticketSegmentEntity.Id}',
	                '{ticketSegmentEntity.TicketId}',
                    {ticketSegmentEntity.SegmentNumber},
	                '{ticketSegmentEntity.TrainCompositionId}',
	                {ticketSegmentEntity.CarNumber},
	                {ticketSegmentEntity.SeatNumber},
                    '{ticketSegmentEntity.DepartureTime}',
                    '{ticketSegmentEntity.ArrivalTime}',
	                '{ticketSegmentEntity.StartStationId}',
	                '{ticketSegmentEntity.EndStationId}'
                );
            ";
        }
    }
}