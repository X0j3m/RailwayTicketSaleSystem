using Contracts.Messages.Query;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Messaging.Sender;

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
            [FromQuery] Guid startStation,
            [FromQuery] Guid endStation,
            [FromQuery] DateTime departureTime)
        {
            var query = new GetTrainConnectionsQuery
            {
                StartStation = startStation,
                EndStation = endStation,
                DepartureTime = departureTime
            };
            _logger.LogInformation("Sending GetTrainConnectionsQuery");
            await _querySender.SendQueryAsync(query);
            return Accepted();
        }
    }
}
