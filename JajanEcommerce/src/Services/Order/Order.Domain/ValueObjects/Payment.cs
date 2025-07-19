namespace Order.Domain.ValueObjects
{
    public enum PaymentMethod
    {
        Cash = 0,
        BankTransfer = 1,
        QRIS = 2
    }

    public record Payment
    {
        public PaymentMethod? PaymentMethod { get; set; } = ValueObjects.PaymentMethod.Cash;
        public decimal CashAmount { get; set; } = 0;
        protected Payment() { }
        protected Payment(PaymentMethod paymentMethod, decimal cashAmount)
        {
            PaymentMethod = paymentMethod;
            CashAmount = cashAmount;
        }

        public static Payment Of(PaymentMethod paymentMethod, decimal amount)
        {
            if (paymentMethod == ValueObjects.PaymentMethod.Cash)
            {
                return new Payment(paymentMethod, amount);
            }
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);

            return new Payment(paymentMethod, 0);
        }
    }
}