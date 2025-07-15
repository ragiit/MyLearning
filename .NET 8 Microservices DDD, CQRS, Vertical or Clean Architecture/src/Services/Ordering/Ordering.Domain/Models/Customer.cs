using Ordering.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Domain.Models
{
    public class Customer : Entity<CustomerId>
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public static Customer Create(CustomerId customerId, string name, string email)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentException.ThrowIfNullOrWhiteSpace(email);

            var customer = new Customer
            {
                Id = customerId,
                Name = name,
                Email = email
            };

            return customer;
        }
    }
}