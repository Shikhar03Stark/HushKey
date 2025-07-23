using Microsoft.AspNetCore.Mvc;

namespace HushKeyApi.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController: ControllerBase
    {
        // GET api/health
        [HttpGet]
        public IActionResult GetHealthStatus()
        {
            return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
        }

        // GET api/health/liveness
        [HttpGet("liveness")]
        public IActionResult GetLiveness()
        {
            return Ok(new { Status = "Alive", Timestamp = DateTime.UtcNow });
        }

        // GET api/health/readiness
        [HttpGet("readiness")]
        public IActionResult GetReadiness()
        {
            // Here you can add checks for database connections, external services, etc.
            // For now, we will just return a simple ready status.
            return Ok(new { Status = "Ready", Timestamp = DateTime.UtcNow });
        }
    }
}
