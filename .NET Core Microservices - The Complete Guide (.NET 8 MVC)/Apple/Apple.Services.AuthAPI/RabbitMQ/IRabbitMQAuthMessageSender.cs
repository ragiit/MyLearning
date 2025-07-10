namespace Apple.Services.AuthAPI.RabbitMQ
{
    public interface IRabbitMQAuthMessageSender
    {
        // Mengirim pesan ke antrian (queue) yang spesifik.
        void SendMessage(object message, string queueName);
    }
}