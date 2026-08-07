using Contracts.Messages.Backend.Command;
using Contracts.Messages.Backend.Query;
using Dapper;
using ReservationService.Model;
using System.Data;

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
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                var ticketId = Guid.NewGuid();

                const string insertTicketSql = INSERT_TICKET_SQL_QUERY;
                await _dbConnection.ExecuteAsync(insertTicketSql, new { TicketId = ticketId }, transaction);

                string insertSegmentSql = INSERT_TICKETS_SEGMENT_SQL_QUERY;

                foreach (var reservation in seatReservations)
                {
                    var entity = new TicketSegmentSqlEntity
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

                    var rowsAffected = await _dbConnection.ExecuteAsync(insertSegmentSql, entity, transaction);

                    if (rowsAffected == 0)
                    {
                        throw new InvalidOperationException(
                            $"Seat {reservation.SeatNumber} in car {reservation.CarNumber} is already reserved in the specified time interval.");
                    }
                }

                transaction.Commit();
                return ticketId;
            }
            catch
            {
                transaction.Rollback();
                return Guid.Empty;
            }
        }

        private const string INSERT_TICKET_SQL_QUERY =
            "INSERT INTO [Tickets] ([id]) VALUES (@TicketId);";

        private const string INSERT_TICKETS_SEGMENT_SQL_QUERY =
            @"
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
                    SELECT 
                        @Id,
                        @TicketId,
                        @SegmentNumber,
                        @TrainCompositionId,
                        @CarNumber,
                        @SeatNumber,
                        @DepartureTime,
                        @ArrivalTime,
                        @StartStationId,
                        @EndStationId
                    WHERE NOT EXISTS (
                        SELECT 1 
                        FROM [TicketSegments] WITH (UPDLOCK, HOLDLOCK)
                        WHERE
                            [train_composition_id] = @TrainCompositionId
                            AND
                            [car_number] = @CarNumber
                            AND
                            [seat_number] = @SeatNumber
                            AND
                            [departure_time] < @ArrivalTime
                            AND
                            [arrival_time] > @DepartureTime)
                ;";
    }
}