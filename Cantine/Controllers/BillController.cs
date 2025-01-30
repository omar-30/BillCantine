using Api.Requests;
using BillingManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cantine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BillController : ControllerBase
    {
        private readonly ILogger<BillController> _logger;
        private readonly IBillService _service;

        public BillController(IBillService service, ILogger<BillController> logger)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet("getBalance/{clientId}")]
        public IActionResult GetBalance(int clientId)
        {
            try
            {
                var client = _service.GetClient(clientId);

                _logger.LogInformation($"récupérer la balance de {client.Name}");
                
                return Ok(new { clientId, client.Balance });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("creditBalance")]
        public IActionResult CreditBalance([FromBody] CreditRequest request)
        {
            _logger.LogInformation($"créditer la balance de ce client avec l'id {request.ClientId}");

            try
            {
                _service.CreditBalance(request.ClientId, request.Amount);
                return Ok(new { message = "Balance credited successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("generateTicket")]
        public IActionResult GenerateTicket([FromBody] TicketRequest request)
        {
            _logger.LogInformation($"Générer un ticket pour le client avec l'id {request.ClientId}");

            try
            {
                var ticket = _service.GenerateTicket(request.ClientId, request.OrderProducts);
                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
