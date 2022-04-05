using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Service;

namespace Scheduler.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ProducerController : ControllerBase
{
    private readonly ILogger<ProducerController> _logger;
    private readonly IRabbitMQService _rabbitmqService;

    public ProducerController(ILogger<ProducerController> logger, IRabbitMQService rabbitMQService)
    {
        _rabbitmqService = rabbitMQService;
        _logger = logger;
    }

    [HttpPost("monitoring")]
    public IActionResult PostMonitoring()
    {
        try
        {
            object exampleObject = new
            {
                Id = Guid.NewGuid().ToString(),
                Name = "PostMonitoringObject",
                Date = DateTime.Now
            };

            _rabbitmqService.SendMessage(exampleObject, "scheduler-ex", "scheduler-monitoring-rk");

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    [HttpPost("status")]
    public IActionResult PostStatus()
    {
        try
        {
            object exampleObject = new
            {
                Id = Guid.NewGuid().ToString(),
                Name = "PostStatusObject",
                Date = DateTime.Now
            };

            _rabbitmqService.SendMessage(exampleObject, "scheduler-ex", "scheduler-status-rk");

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Endpoint responsável por acionar um Agent com configuração específica.
    /// </summary>
    /// <param name="taskConfig"></param>
    /// <returns></returns>
    [HttpPost("submition")]
    public IActionResult PostSubmition(TaskConfig taskConfig)
    {
        try
        {
            _rabbitmqService.SendMessage(taskConfig, "scheduler-ex", "scheduler-submition-rk");

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}