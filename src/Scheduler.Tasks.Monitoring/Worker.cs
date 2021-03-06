using Scheduler.API.Service;

namespace Scheduler.Worker.Monitoring;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IRabbitMQService _rabbitMQService;

    public Worker(ILogger<Worker> logger, IRabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _rabbitMQService.ConsumeMessage<object>("scheduler-monitoring-qu", (message) =>
        {
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}