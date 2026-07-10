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
        public async Task<IActionResult> GetTrainCompositions(Guid id)
        {
            string requestId = id.ToString();
            var query = new GetAvailableSeatsQuery
            {
                TrainCompositionId = id,
                StartStation = Guid.NewGuid(), // Replace with actual start station ID
                EndStation = Guid.NewGuid() // Replace with actual end station ID
            };
            _logger.LogInformation("Sending GetAvailableSeatsQuery");
            await _querySender.SendQueryAsync(query);
            return Accepted();
        }
    }
}
