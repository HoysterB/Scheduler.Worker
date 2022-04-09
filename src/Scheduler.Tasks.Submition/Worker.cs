using Scheduler.API.Service;
using Scheduler.Core;
using System.Threading.Tasks;

namespace Scheduler.Worker.Submition;

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
        _rabbitMQService.ConsumerMessage<TaskConfig>("scheduler-submition-qu", (taskConfig) =>
        {
            // Definir o que acontecerá após receber mensagem da fila!

            var agent = StrategyMapping.ComponentAgentMapping[taskConfig.Component];
            agent.Init(Guid.NewGuid(), taskConfig);
            agent.Evaluate();
        });

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}