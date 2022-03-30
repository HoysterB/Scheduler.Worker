namespace Scheduler.API.Service;

public interface IRabbitMQService
{
    bool SendMessage(object request, string exchange, string routingKey);
}
