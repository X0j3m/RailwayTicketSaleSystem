using Contracts.Query;
using Microsoft.AspNetCore.Mvc;
using WebAPI.QueueHandler.Send;

namespace WebAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainCompositionController : ControllerBase
    {
        private readonly ILogger<TrainCompositionController> _logger;
        private readonly QuerySender _querySender;

        public TrainCompositionController(ILogger<TrainCompositionController> logger, QuerySender querySender)
        {
            _logger = logger;
            _querySender = querySender;
        }

        [HttpGet]
        [Route("{id}/available-seats")]
        public async Task<IActionResult> GetTrainCompositions(
            Guid id,
            [FromQuery] Guid startStationId,
            [FromQuery] Guid endStationId)
        {
            string requestId = id.ToString();
            var query = new GetAvailableSeatsQuery
            {
                TrainCompositionId = id,
                StartStation = startStationId,
                EndStation = endStationId
            };
            _logger.LogInformation("Sending GetAvailableSeatsQuery");
            await _querySender.SendQueryAsync(query);
            return Accepted();
        }
    }
}
