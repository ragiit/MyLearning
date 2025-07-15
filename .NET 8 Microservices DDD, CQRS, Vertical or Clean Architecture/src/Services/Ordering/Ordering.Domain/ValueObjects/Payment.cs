using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.ValueObjects
{
    public record Payment
    {
        public string? CardName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string Expiration { get; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public int PaymentMethod { get; set; } = 0;

        protected Payment() { }

        public Payment(string? cardName, string cardNumber, string expiration, string cVV, int paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CVV = cVV;
            PaymentMethod = paymentMethod;
        }

        public static Payment Of(string? cardName, string cardNumber, string expiration, string cVV, int paymentMethod)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName);
            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(cVV);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cVV.Length, 3);

            return new Payment(cardName, cardNumber, expiration, cVV, paymentMethod);
        }
    }
}