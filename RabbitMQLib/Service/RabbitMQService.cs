using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Scheduler.API.Service;

public class RabbitMQService :  IRabbitMQService
{
    #region Properties

    private readonly ILogger _logger;
    private IConnection _conn;
    private IModel _channel;

    #endregion Properties

    #region Constructors

    public RabbitMQService(ILogger<RabbitMQService> logger)
    {
        _logger = logger;
        _logger.LogInformation($"[RabbitMQService] RabbitMQ connected at: {DateTime.UtcNow.ToLongTimeString()}");
    }

    #endregion Constructors

    #region Methods

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

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[RabbitMQService] Exception in CreateConnection");
            return false;
        }
    }

    public bool CreateModel()
    {
        try
        {
            CheckConnection();

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
            CheckConnection();

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
            _logger.LogError(e, $"[RabbitMQService] Exception in SendMessage");

            return false;
        }
    }

    public bool CreateQueue(string queueName, string exchange, string routingKey)
    {
        try
        {
            CheckConnection();

            string queueTag = _channel.QueueDeclare(queue: queueName,
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            _logger.LogInformation($"[RabbitMQService] The queue {queueName} was created successfully!");
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[RabbitMQService] Exception in QueueCreate");

            return false;
        }
    }

    public bool CreateBindQueue(string queueName, string exchange, string routingKey)
    {
        try
        {            
            CheckConnection();

            _channel.QueueBind(queue: queueName, exchange: exchange, routingKey: routingKey);

            _logger.LogInformation($"[RabbitMQService] The bind from Queue: {queueName} to Routing Key: {routingKey} was created successfully!");

            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"[RabbitMQService] Exception in QueueBind");

            return false;
        }
    }

    public bool CreateExchange(string exchangeName, string type)
    {
        try
        {
            CheckConnection();

            _channel.ExchangeDeclare(exchangeName, type);

            _logger.LogInformation($"[RabbitMQService] The Exchange: {exchangeName} was created successfully!");

            return true;
        }
        catch (Exception e)
        {
            _logger.LogInformation(e, "[RabbitMQService] Exception in ExchangeCreate");

            return false;
        }
    }

    public bool ConsumeMessage<T>(string queueName, Action<T> procedure)
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

    private void CheckConnection()
    {
        try
        {
            if (_conn.IsOpen == false)
            {
                throw new Exception("RabbitMQ connection is not open");
            }
        }
        catch (Exception e)
        {
            throw new Exception("RabbitMQ connection problem");
        }
    }

    #endregion Methods
}