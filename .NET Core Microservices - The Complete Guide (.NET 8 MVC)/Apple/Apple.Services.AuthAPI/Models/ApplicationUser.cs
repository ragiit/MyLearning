﻿using Microsoft.AspNetCore.Identity;

namespace Apple.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}