using Microsoft.AspNetCore.Mvc;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/blockchain")]
    public class BlockchainController : ControllerBase
    {
        private readonly ILogger<BlockchainController> _logger;

        public BlockchainController(ILogger<BlockchainController> logger)
        {
            _logger = logger;
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetBlockchainTransactions()
        {
            try
            {
                var transactions = new
                {
                    total_transactions = 8945,
                    pending_transactions = 12,
                    block_height = 15678,
                    network_status = "Healthy",
                    recent_transactions = new[]
                    {
                        new
                        {
                            hash = "0x1a2b3c4d5e6f7890",
                            type = "Attendance Record",
                            timestamp = "2024-06-27T10:30:00Z",
                            status = "Confirmed"
                        },
                        new
                        {
                            hash = "0x9876543210abcdef",
                            type = "Payroll Transaction",
                            timestamp = "2024-06-27T09:15:00Z",
                            status = "Confirmed"
                        }
                    }
                };
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blockchain transactions");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
