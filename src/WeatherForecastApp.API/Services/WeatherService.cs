using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WeatherForecastApp.API.Services
{
    public class WeatherService:BackgroundService
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly EventingBasicConsumer consumer;
        public WeatherService()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (connection = factory.CreateConnection())
            using (channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                consumer = new EventingBasicConsumer(channel);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            consumer.Received += (model, content) =>
            {
                var body = content.Body.ToArray();
                var result = Encoding.UTF8.GetString(body);
            };

            channel.BasicConsume("hello", true, consumer);
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}