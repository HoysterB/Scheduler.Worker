
namespace Scheduler.API.Service
{
    public interface IRabbitMQService
    {
        bool ConsumerMessage<T>(string queueName, Action<T> procedure);
        bool CreateConnection();
        bool ExchangeCreate(string exchangeName, string type);
        bool QueueCreate(string queueName, string exchange, string routingKey);
        bool SendMessage(object request, string exchange, string routingKey);
    }
}