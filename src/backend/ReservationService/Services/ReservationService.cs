using Contracts.Messages.Backend.Command;
using Dapper;
using Microsoft.Data.SqlClient;
using Models.Dtos;
using ReservationService.Model;
using System.Data;

namespace ReservationService.Services
{
    public class ReservationService
    {
        private readonly ILogger<ReservationService> _logger;
        private readonly string _connectionString;

        public ReservationService(ILogger<ReservationService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("MicrosoftSQLServer")
                ?? throw new ArgumentNullException("MicrosoftSQLServer");
        }

        public async Task<TicketDto[]> GetTicketsByEmailAsync(string email, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching tickets for email: {Email}", email);

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync(cancellationToken);

                var getTicketsCmd = new CommandDefinition(
                    commandText: GET_TICKETS_BY_EMAIL_SQL_QUERY,
                    parameters: new { Email = email },
                    commandTimeout: 5,
                    cancellationToken: cancellationToken
                );

                var tickets = await connection.QueryAsync<TicketDto>(getTicketsCmd);
                var result = tickets.ToArray();

                _logger.LogInformation("Fetched {Count} tickets for email: {Email}", result.Length, email);
                return result;
            }
            catch (SqlException ex) when (ex.Number == -2)
            {
                _logger.LogWarning(ex, "SQL Timeout while fetching tickets for email: {Email}", email);
                throw new TimeoutException($"Database query timed out for email: {email}", ex);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                _logger.LogWarning("Fetching tickets for email {Email} was canceled by MassTransit timeout.", email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while fetching tickets for email: {Email}", email);
                throw;
            }
        }

        public async Task<Guid> CancelReservationAsync(Guid ticketId, CancellationToken cancellationToken)
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            try
            {
                var cancelTicketCmd = new CommandDefinition(
                    commandText: CANCEL_TICKET_SQL_QUERY,
                    parameters: new { TicketId = ticketId },
                    transaction: transaction,
                    commandTimeout: 5,
                    cancellationToken: cancellationToken
                );

                await connection.ExecuteAsync(cancelTicketCmd);
                await transaction.CommitAsync(cancellationToken);
                return ticketId;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Reservation cancellation cancelled due to timeout/cancellation token. Rolling back.");
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling reservation. Performing Rollback.");
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        public async Task<Guid> CreateReservationAsync(string email, SeatReservation[] seatReservations, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating reservation for {Count} seat reservations.", seatReservations?.Length ?? 0);

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            await using var transaction = (SqlTransaction)await connection.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);

            try
            {
                var ticketId = Guid.NewGuid();
                _logger.LogInformation("Generated TicketId: {TicketId}", ticketId);

                var insertTicketCmd = new CommandDefinition(
                    commandText: INSERT_TICKET_SQL_QUERY,
                    parameters: new { TicketId = ticketId, Email = email },
                    transaction: transaction,
                    commandTimeout: 5,
                    cancellationToken: cancellationToken
                );
                await connection.ExecuteAsync(insertTicketCmd);

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

                    var insertSegmentCmd = new CommandDefinition(
                        commandText: INSERT_TICKETS_SEGMENT_SQL_QUERY,
                        parameters: entity,
                        transaction: transaction,
                        commandTimeout: 5,
                        cancellationToken: cancellationToken
                    );

                    var rowsAffected = await connection.ExecuteAsync(insertSegmentCmd);

                    if (rowsAffected == 0)
                    {
                        _logger.LogWarning("RESERVATION COLLISION! Seat {Seat} in car {Car} is already reserved.", entity.SeatNumber, entity.CarNumber);
                        throw new InvalidOperationException($"Seat {reservation.SeatNumber} in car {reservation.CarNumber} is already reserved.");
                    }
                }

                await transaction.CommitAsync(cancellationToken);
                _logger.LogInformation("Reservation committed successfully for TicketId: {TicketId}", ticketId);
                return ticketId;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Reservation cancelled due to timeout/cancellation token. Rolling back.");
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating reservation. Performing Rollback.");
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }

        private const string CANCEL_TICKET_SQL_QUERY =
            @"
                UPDATE [Tickets]
                SET [status] = 'CANCELED'
                WHERE [id] = @TicketId
            ";

        private const string INSERT_TICKET_SQL_QUERY =
            "INSERT INTO [Tickets] ([id], [email], [status]) VALUES (@TicketId, @Email, 'CREATED');";

        private const string INSERT_TICKETS_SEGMENT_SQL_QUERY =
            @"
                INSERT INTO [TicketSegments] (
                    [id], [ticket_id], [segment_number], [train_composition_id],
                    [car_number], [seat_number], [departure_time], [arrival_time],
                    [start_station_id], [end_station_id])
                SELECT 
                    @Id, @TicketId, @SegmentNumber, @TrainCompositionId,
                    @CarNumber, @SeatNumber, @DepartureTime, @ArrivalTime,
                    @StartStationId, @EndStationId
                WHERE NOT EXISTS (
                    SELECT 1 
                    FROM [TicketSegments] WITH (UPDLOCK, HOLDLOCK)
                    WHERE
                        [train_composition_id] = @TrainCompositionId
                        AND [car_number] = @CarNumber
                        AND [seat_number] = @SeatNumber
                        AND [departure_time] < @ArrivalTime
                        AND [arrival_time] > @DepartureTime
                );";

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
                    WHERE t.[email] = @Email
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

    }
}