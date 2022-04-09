using Microsoft.AspNetCore.Mvc;
using Scheduler.API.Service;
using Scheduler.Core;

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
            bool response = _rabbitmqService.SendMessage(taskConfig, "scheduler-ex", "scheduler-submition-rk");

            if (response)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, "RabbitMQ Problem");
            }
        }
        catch (Exception)
        {
            return StatusCode(500);
        }
    }
}