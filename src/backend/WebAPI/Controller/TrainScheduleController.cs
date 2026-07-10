using Contracts.Query;
using Microsoft.AspNetCore.Mvc;
using WebAPI.QueueHandler.Send;

namespace WebAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainScheduleController : ControllerBase
    {
        private readonly QuerySender _querySender;
        private readonly ILogger<TrainScheduleController> _logger;

        public TrainScheduleController(QuerySender querySender, ILogger<TrainScheduleController> logger)
        {
            _querySender = querySender;
            _logger = logger;
        }

        [HttpGet]
        [Route("/connections")]
        public async Task<IActionResult> GetTrainConnections(
            //[FromQuery] Guid startStation,
            //[FromQuery] Guid endStation,
            //[FromQuery] DateTimeOffset departureTime
            )
        {
            var query = new GetTrainConnectionsQuery
            {
                StartStation = Guid.NewGuid(), // Replace with actual start station ID
                EndStation = Guid.NewGuid(), // Replace with actual end station ID
                DepartureTime = DateTime.UtcNow.AddDays(1) // Replace with actual departure time
            };
            _logger.LogInformation("Sending GetTrainConnectionsQuery");
            await _querySender.SendQueryAsync(query);
            return Accepted();
        }
    }
}
