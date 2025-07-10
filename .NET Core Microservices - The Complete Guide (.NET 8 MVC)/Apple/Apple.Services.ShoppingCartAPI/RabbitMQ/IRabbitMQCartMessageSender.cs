namespace Apple.Services.ShoppingCartAPI.RabbitMQ
{
    public interface IRabbitMQCartMessageSender
    {
        // Mengirim pesan ke antrian (queue) yang spesifik.
        void SendMessage(object message, string queueName);
    }
}