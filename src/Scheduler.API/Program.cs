using Scheduler.API.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>(sp =>
{
    var rabbitMQ = new RabbitMQService(sp.GetService<ILogger<RabbitMQService>>());
    if (rabbitMQ.CreateConnection() && rabbitMQ.CreateModel())
    {
        string exchangeName = "scheduler-ex";
        rabbitMQ.CreateExchange(exchangeName, "topic");

        //rabbitMQ.QueueCreate("scheduler-status-qu", exchangeName, "scheduler-status-rk");
        //rabbitMQ.QueueBind("scheduler-status-qu", exchangeName, "scheduler-status-rk");

        //rabbitMQ.QueueCreate("scheduler-monitoring-qu", exchangeName, "scheduler-monitoring-rk");
        //rabbitMQ.QueueBind("scheduler-monitoring-qu", exchangeName, "scheduler-monitoring-rk");

        rabbitMQ.CreateQueue("scheduler-submition-qu", exchangeName, "scheduler-submition-rk");
        rabbitMQ.CreateBindQueue("scheduler-submition-qu", exchangeName, "scheduler-submition-rk");
    }
    return rabbitMQ;
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();