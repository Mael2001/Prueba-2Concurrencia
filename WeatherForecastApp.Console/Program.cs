using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace WeatherForecastApp.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Ingresar Fecha a buscar formato (yyyyMMdd)");
            string date = System.Console.ReadLine();

            var json = JsonConvert.SerializeObject(date);
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "hello",
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "",
                        routingKey: "hello",
                        basicProperties: null,
                        body: body);
                    System.Console.WriteLine(" [x] Sent {0}", json);
                    //----------------------------------------


                }
            System.Console.WriteLine(" Press [enter] to exit.");
            System.Console.ReadLine();
        }
    }
}
