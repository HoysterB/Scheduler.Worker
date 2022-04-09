
namespace Scheduler.API.Service
{
    public interface IRabbitMQService
    {
        bool ConsumeMessage<T>(string queueName, Action<T> procedure);
        bool CreateConnection();
        bool CreateModel();
        bool CreateExchange(string exchangeName, string type);
        bool CreateBindQueue(string queueName, string exchange, string routingKey);
        bool CreateQueue(string queueName, string exchange, string routingKey);
        bool SendMessage(object request, string exchange, string routingKey);
    }
}