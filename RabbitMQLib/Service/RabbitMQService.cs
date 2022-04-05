using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Scheduler.API.Service;

public class RabbitMQService : IRabbitMQService
{
    private readonly ILogger _logger;
    private IConnection _conn;
    private IModel _channel;

    public RabbitMQService(ILogger<RabbitMQService> logger)
    {
        _logger = logger;
    }

    public bool CreateConnection()
    {
        try
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                Port = 5672,
                UserName = "guest",
                Password = "guest"
            };
            _conn = factory.CreateConnection();
            _channel = _conn.CreateModel();
            _logger.LogInformation($"[RabbitMQService] RabbitMQ connected at: {DateTime.UtcNow.ToLongTimeString()}");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[RabbitMQService] Exception in CreateConnection");
            return false;
        }
    }

    public bool SendMessage(object request, string exchange, string routingKey)
    {
        try
        {
            var objectJson = System.Text.Json.JsonSerializer.Serialize(request);
            var body = Encoding.UTF8.GetBytes(objectJson);

            _channel.BasicPublish(exchange: exchange,
                                 routingKey: routingKey,
                                 basicProperties: null,
                                 body: body);

            _logger.LogInformation($"[RabbitMQService] published at exchange: {exchange} - routing key: {routingKey}");

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[RabbitMQService] Exception in Constructor");

            return false;
        }
    }

    public bool QueueCreate(string queueName, string exchange, string routingKey)
    {
        try
        {
            _channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
            _channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[RabbitMQService] Exception in Constructor");

            return false;
        }
    }

    public bool ExchangeCreate(string exchangeName, string type)
    {
        try
        {
            _channel.ExchangeDeclare(exchangeName, type);

            return true;
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, "[RabbitMQService] Exception in CreateExchange");

            return false;
        }
    }

    public bool ConsumerMessage<T>(string queueName, Action<T> procedure)
    {
        try
        {
            IModel channel = _conn.CreateModel();
            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _logger.LogInformation($"[RabbitMQService] Reading message...");

                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        procedure.Invoke(JsonConvert.DeserializeObject<T>(message));
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"[RabbitMQService] Attemp to read message from queue error");
                    }
                });
            };

            string consumerTag = channel.BasicConsume(queue: queueName,
                                   autoAck: true,
                                   consumer: consumer);

            if (string.IsNullOrEmpty(consumerTag)) return false;

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[RabbitMQService] Exception in ConsumerMessage");
            return false;
        }
    }
}