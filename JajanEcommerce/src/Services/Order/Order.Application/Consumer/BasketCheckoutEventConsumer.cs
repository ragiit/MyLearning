using BuildingBlocks.Messaging.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Domain.Repositories;
using Order.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Application.Consumer
{
    public class BasketCheckoutEventConsumer(IMediator mediator, ILogger<BasketCheckoutEventConsumer> logger, IOrderRepository _orderRepository)
       : IConsumer<BasketCheckoutEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var message = context.Message;

            logger.LogInformation("Received BasketCheckoutEvent for user {UserId}", message.Id);

            // Konversi ke ValueObjects
            var customerId = CustomerId.Of(message.Username); // Asumsikan CustomerId berbasis string username
            var orderId = OrderId.Of(Guid.NewGuid());
            var orderName = OrderName.Of($"Order - {message.Date:yyyyMMdd-HHmmss}");

            // Buat payment object berdasarkan metode
            var payment = Payment.Of((PaymentMethod)message.PaymentMethod, message.CashAmount);

            // Buat entitas Order
            var order = Order.Domain.Models.Order.Create(orderId, customerId, orderName, payment);

            // Tambahkan item dari event ke dalam Order
            foreach (var item in message.Items)
            {
                // Harga perlu dicari ulang dari MenuItem service (jika tidak disediakan dalam event)
                decimal dummyPrice = 10000; // Contoh sementara
                string dummyProductName = "Menu"; // Contoh sementara

                order.AddItem(
                    MenuItemId.Of(item.MenuItemId),
                    item.ProductName,
                    item.Quantity,
                    item.Price
                );
            }

            // bisa dirubah nanti fungsi ini
            order.CreatedAt = DateTime.UtcNow;
            order.CreatedBy = message.Username;

            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();
        }
    }
}