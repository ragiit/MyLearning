using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.ValueObjects
{
    public record Address
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string AddressLine { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;

        protected Address() { }

        public Address(string firstName, string lastName, string? email, string addressLine, string country, string state, string zipCode)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            AddressLine = addressLine;
            Country = country;
            State = state;
            ZipCode = zipCode;
        }

        public static Address Of(string firstName, string lastName, string? email, string addressLine, string country, string state, string zipCode)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(email);
            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine);

            return new Address(firstName, lastName, email, addressLine, country, state, zipCode);
        }
    }
}