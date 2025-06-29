using Microsoft.AspNetCore.Mvc;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpPost("input")]
        public IActionResult TestInput([FromBody] TestInputModel model)
        {
            return Ok(new { message = "Input processed", data = model });
        }
    }

    public class TestInputModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
