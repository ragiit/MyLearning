﻿using Bulky.Models.Models;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader orderHeader);

        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);

        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
    }
}