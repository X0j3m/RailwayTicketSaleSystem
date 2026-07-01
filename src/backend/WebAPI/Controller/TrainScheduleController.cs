using Contracts.Query;
using Microsoft.AspNetCore.Mvc;
using WebAPI.QueueSender;

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
        [Route("stations")]
        public async Task<IActionResult> GetStations()
        {
            var query = new GetStationsQuery();
            _logger.LogInformation("Sending GetStationsQuery");
            await _querySender.SendQueryAsync(query);
            return Accepted();
        }
    }
}
