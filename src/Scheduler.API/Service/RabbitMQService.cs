using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Scheduler.API.Config;

namespace Scheduler.API.Service;

public class RabbitMQService : IRabbitMQService
{
    private readonly IConnection _conn;
    private readonly IModel _channel;
    private readonly RabbitMQServiceSettings _rabbitMQSettings;
    private Guid _id;

    public RabbitMQService(IOptions<RabbitMQServiceSettings> options)
    {

        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = _rabbitMQSettings.Hostname,
                Port = Convert.ToInt32(_rabbitMQSettings.Port),
                UserName = _rabbitMQSettings.Username,
                Password = _rabbitMQSettings.Password
            };
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();

            if (_conn.IsOpen)
            {
                _channel.ExchangeDeclare(_rabbitMQSettings.Exchange, "topic");

                // Queue Send Email Payment
                _channel.QueueDeclare(queue: "send-payment-email",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                _channel.QueueBind(queue: "send-payment-email", exchange: _rabbitMQSettings.Exchange, routingKey: "send-payment-email-rk");

                // Queue Send Email Newsletter
                _channel.QueueDeclare(queue: "send-newsletter-email",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                _channel.QueueBind(queue: "send-newsletter-email", exchange: _rabbitMQSettings.Exchange, routingKey: "send-newsletter-email-rk");
            }

            Console.WriteLine("Exchange e Queues criadas com sucesso!");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message, "Fatal error");
        }
    }

    public bool SendMessage(object request, string exchange, string routingKey)
    {
        var objectJson = System.Text.Json.JsonSerializer.Serialize(request);
        var body = Encoding.UTF8.GetBytes(objectJson);

        _channel.BasicPublish(exchange: exchange,
                             routingKey: routingKey,
                             basicProperties: null,
                             body: body);

        return true;
    }
}