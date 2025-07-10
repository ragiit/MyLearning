using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Apple.Services.EmailAPI.Messaging
{
    public class RabbitMqAuthConsumer(IConfiguration configuration, IServiceScopeFactory scopeFactory) : IHostedService
    {
        private IConnection _connection;
        private RabbitMQ.Client.IModel _channel;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var factory = new ConnectionFactory
                {
                    HostName = configuration.GetValue<string>("RabbitMQ:HostName"),
                    UserName = configuration.GetValue<string>("RabbitMQ:UserName"),
                    Password = configuration.GetValue<string>("RabbitMQ:Password")
                };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                var queueName = "register.queue";
                _channel.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += OnMessageReceived;

                _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return Task.CompletedTask;
        }

        private async void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Body.ToArray());
            var userDto = JsonConvert.DeserializeObject<dynamic>(message);

            try
            {
                // Membuat scope baru untuk mendapatkan service (praktik terbaik di background service)
                using var scope = scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                if (userDto != null)
                {
                    await emailService.RegisterUserEmailAndLog(userDto);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection?.Close();
            return Task.CompletedTask;
        }
    }
}