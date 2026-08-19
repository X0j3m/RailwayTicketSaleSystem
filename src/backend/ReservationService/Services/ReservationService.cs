using Contracts.Messages.Backend.Command;
using Dapper;
using Models.Dtos;
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

        public async Task<TicketDto[]> GetTicketsByEmailAsync(string email)
        {
            _logger.LogInformation($"Fetching tickets for email: {email}");
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            try
            {
                var query = GET_TICKETS_BY_EMAIL_SQL_QUERY.Replace("@Email", email);
                var tickets = await _dbConnection.QueryAsync<TicketDto>(query);
                _logger.LogInformation($"Fetched {tickets.AsList().Count} tickets for email: {email}");
                return tickets.AsList().ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while fetching tickets for email: {email}");
                return [];
            }
        }

        public async Task<Guid> CancelReservationAsync(Guid ticketId)
        {
            _logger.LogInformation($"Cancelling reservation for TicketId: {ticketId}");
            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }
            using var transaction = _dbConnection.BeginTransaction();
            try
            {
                const string deleteSegmentsSql = "DELETE FROM [TicketSegments] WHERE [ticket_id] = @TicketId;";
                await _dbConnection.ExecuteAsync(deleteSegmentsSql, new { TicketId = ticketId }, transaction);
                const string deleteTicketSql = "DELETE FROM [Tickets] WHERE [id] = @TicketId;";
                await _dbConnection.ExecuteAsync(deleteTicketSql, new { TicketId = ticketId }, transaction);
                transaction.Commit();
                _logger.LogInformation($"Reservation cancelled successfully for TicketId: {ticketId}");
                return ticketId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while cancelling reservation for TicketId: {ticketId}. Performing Rollback.");
                transaction.Rollback();
                return Guid.Empty;
            }
        }

        public async Task<Guid> CreateReservationAsync(string email, SeatReservation[] seatReservations)
        {
            _logger.LogInformation($"Creating reservation for {seatReservations?.Length ?? 0} seat reservations.");

            if (_dbConnection.State != ConnectionState.Open)
            {
                _dbConnection.Open();
            }

            using var transaction = _dbConnection.BeginTransaction();

            try
            {
                var ticketId = Guid.NewGuid();
                _logger.LogInformation($"Generated TicketId: {ticketId}");

                const string insertTicketSql = INSERT_TICKET_SQL_QUERY;
                await _dbConnection.ExecuteAsync(insertTicketSql, new { TicketId = ticketId, Email = email }, transaction);

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

                    _logger.LogInformation(
                        $"Trying to insert segment: Id: {entity.Id}, TicketId: {entity.TicketId}, SegmentNumber: {entity.SegmentNumber}, " +
                        $"TrainCompositionId: {entity.TrainCompositionId}, CarNumber: {entity.CarNumber}, SeatNumber: {entity.SeatNumber}, " +
                        $"DepartureTime: {entity.DepartureTime:yyyy-MM-dd HH:mm:ss.fff}, ArrivalTime: {entity.ArrivalTime:yyyy-MM-dd HH:mm:ss.fff}, " +
                        $"StartStationId: {entity.StartStationId}, EndStationId: {entity.EndStationId}");

                    var rowsAffected = await _dbConnection.ExecuteAsync(insertSegmentSql, entity, transaction);

                    if (rowsAffected == 0)
                    {
                        _logger.LogWarning(
                            $"RESERVATION COLLISION! Segment not saved. Seat {entity.SeatNumber} in car {entity.CarNumber} is already reserved " +
                            $"for the time interval {entity.DepartureTime:yyyy-MM-dd HH:mm:ss.fff} - {entity.ArrivalTime:yyyy-MM-dd HH:mm:ss.fff} " +
                            $"for train composition {entity.TrainCompositionId}.");

                        throw new InvalidOperationException(
                            $"Seat {reservation.SeatNumber} in car {reservation.CarNumber} is already reserved in the specified time interval.");
                    }
                }

                transaction.Commit();
                _logger.LogInformation($"Reservation transaction committed successfully for TicketId: {ticketId}");
                return ticketId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating reservation. Performing Rollback.");
                transaction.Rollback();
                return Guid.Empty;
            }
        }

        private const string GET_TICKETS_BY_EMAIL_SQL_QUERY =
            @"
                SET DATEFORMAT ymd;
                WITH RankedSegments AS (
                    SELECT 
                        ts.[ticket_id],
                        ts.[start_station_id],
                        ts.[end_station_id],
                        ts.[departure_time],
                        ts.[arrival_time],
                        ROW_NUMBER() OVER (
                            PARTITION BY ts.[ticket_id] 
                            ORDER BY ts.[segment_number] ASC, ts.[departure_time] ASC
                        ) AS rn_first,
                        ROW_NUMBER() OVER (
                            PARTITION BY ts.[ticket_id] 
                            ORDER BY ts.[segment_number] DESC, ts.[arrival_time] DESC
                        ) AS rn_last
                    FROM [TicketSegments] ts
                    JOIN [Tickets] t ON t.[id] = ts.[ticket_id]
                    WHERE t.[email] = '@Email'
                )
                SELECT 
                    [ticket_id] AS TicketId,
                    MIN([departure_time]) AS DepartureTime,
                    MAX([arrival_time])   AS ArrivalTime,
                    MAX(CASE WHEN rn_first = 1 THEN [start_station_id] END) AS FromStationId,
                    MAX(CASE WHEN rn_last  = 1 THEN [end_station_id]   END) AS ToStationId
                FROM RankedSegments
                GROUP BY [ticket_id];
            ";

        private const string INSERT_TICKET_SQL_QUERY =
            "INSERT INTO [Tickets] ([id], [email]) VALUES (@TicketId, @Email);";

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