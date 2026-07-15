using Contracts.Messages.Command;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Messaging.Sender;

namespace WebAPI.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ILogger<ReservationsController> _logger;
        private readonly CommandSender _commandSender;

        public ReservationsController(ILogger<ReservationsController> logger, CommandSender commandSender)
        {
            _logger = logger;
            _commandSender = commandSender;
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationCommand reservation)
        {
            await _commandSender.SendCommandAsync(reservation);
            return Accepted();
        }

        [HttpPost]
        [Route("cancel")]
        public async Task<IActionResult> CancelReservation([FromBody] CancelReservationCommand cancelReservation)
        {
            await _commandSender.SendCommandAsync(cancelReservation);
            return Accepted();
        }
    }
}