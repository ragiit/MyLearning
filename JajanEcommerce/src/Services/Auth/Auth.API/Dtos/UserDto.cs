﻿namespace Auth.API.Dtos
{
    public sealed record UserDto
    {
        public string ID { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}