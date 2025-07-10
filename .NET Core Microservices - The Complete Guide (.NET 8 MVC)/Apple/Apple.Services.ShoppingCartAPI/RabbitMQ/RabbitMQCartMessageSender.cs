using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Apple.Services.ShoppingCartAPI.RabbitMQ
{
    public class RabbitMQCartMessageSender(IConfiguration configuration) : IRabbitMQCartMessageSender
    {
        private IConnection? _connection;
        private readonly IConfiguration _configuration = configuration;

        public void SendMessage(object message, string queueName)
        {
            try
            {
                if (!ConnectionExists())
                {
                    Console.WriteLine("RabbitMQ connection not available.");
                    return;
                }

                using var channel = _connection!.CreateModel();

                // Declare queue
                channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                // Serialize message
                var jsonMessage = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(jsonMessage);

                // Publish message
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                Console.WriteLine($"Message published to queue {queueName}: {jsonMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send message: {ex.Message}");
            }
        }

        private void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                    UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                    Password = _configuration["RabbitMQ:Password"] ?? "guest"
                };

                // Gunakan versi sync
                _connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
            }
        }

        private bool ConnectionExists()
        {
            if (_connection == null || !_connection.IsOpen)
            {
                CreateConnection();
            }

            return _connection != null && _connection.IsOpen;
        }
    }
}