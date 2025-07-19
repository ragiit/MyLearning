using Order.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Domain.ValueObjects
{
    public record CustomerId
    {
        public string Value { get; }

        private CustomerId(string value) => Value = value;
        public static CustomerId Of(string value)
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value == string.Empty)
            {
                throw new DomainException("CustomerId cannot be empty.");
            }

            return new CustomerId(value);
        }
    }
}