using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Prueba
{
    class Program
    {
        private static readonly HttpClient _httpClient;
        static async Task Main(string[] args)
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    message = message.Replace("\"", "");
                    Console.WriteLine(" [x] Received {0}", message);
                    using (var httpClient = new HttpClient())
                    {
                        var URL = "http://localhost:59165/weatherforecast/"+message;
                        Console.WriteLine(URL);
                        var response = await httpClient.GetStringAsync($"{URL}");
                        Thread.Sleep(1000);
                        Console.WriteLine(response);

                        /*
                        var json = JsonConvert.SerializeObject(response);
                        channel.QueueDeclare(queue: "response",
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        var data = Encoding.UTF8.GetBytes(json);

                        channel.BasicPublish(exchange: "",
                            routingKey: "response",
                            basicProperties: null,
                            body: data);
                        System.Console.WriteLine(" [x] Sent {0}" );*/
                    }

                };
                channel.BasicConsume(queue: "hello",
                    autoAck: true,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
