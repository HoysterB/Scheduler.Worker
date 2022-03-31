using Scheduler.API.Service;
using Scheduler.Tasks.Monitoring;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IRabbitMQService, RabbitMQService>(sp => {

            var rabbitMQ = new RabbitMQService(sp.GetService<ILogger<RabbitMQService>>());
            if (rabbitMQ.CreateConnection())
            {
                Console.WriteLine("Worker connected RabbitMQ");
            }
            return rabbitMQ;
        });

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
