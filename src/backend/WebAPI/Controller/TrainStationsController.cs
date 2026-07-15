using Contracts.Messages.Query;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Messaging.Sender;

namespace WebAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainStationsController : ControllerBase
    {
        private readonly QuerySender _querySender;
        private readonly ILogger<TrainStationsController> _logger;

        public TrainStationsController(QuerySender querySender, ILogger<TrainStationsController> logger)
        {
            _querySender = querySender;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetStationsQuery();
            _logger.LogInformation("Sending GetStationsQuery");
            await _querySender.SendQueryAsync(query);
            return Accepted();
        }
    }
}
