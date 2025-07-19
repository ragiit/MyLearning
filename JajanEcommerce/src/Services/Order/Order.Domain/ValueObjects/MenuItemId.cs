﻿namespace Order.Domain.ValueObjects
{
    public record MenuItemId
    {
        public Guid Value { get; }

        private MenuItemId(Guid value) => Value = value;
        public static MenuItemId Of(Guid value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == Guid.Empty)
            {
                throw new DomainException("MenuItemId cannot be empty.");
            }

            return new MenuItemId(value);
        }
    }
}