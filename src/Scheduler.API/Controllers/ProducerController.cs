using Microsoft.AspNetCore.Mvc;

namespace Scheduler.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
 
    private readonly ILogger<ProducerController> _logger;

    public ProducerController(ILogger<ProducerController> logger)
    {
        _logger = logger;
    }

    [HttpPost("monitoring")]
    public IActionResult PostMonitoring()
    {
        return Ok();
    }

    [HttpPost("status")]
    public IActionResult PostStatus()
    {
        return Ok();
    }

    [HttpPost("submition")]
    public IActionResult PostSubmition()
    {
        return Ok();
    }

}
