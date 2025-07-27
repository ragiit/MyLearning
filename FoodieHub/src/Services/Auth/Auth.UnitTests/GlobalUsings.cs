// Auth.Application.UnitTests/Usings.cs
global using Auth.API.Dtos;
global using Auth.API.Exceptions;
global using Auth.API.Features.Login; // Untuk Login Command/Handler
global using Auth.API.Persistence;
global using Auth.API.Persistence.Entities;
global using Auth.API.Services.IServices;
global using Auth.UnitTests.Mocks;

global using FluentValidation.TestHelper; // Untuk pengujian FluentValidation
global using Microsoft.AspNetCore.Identity; // Untuk UserManager, SignInManager
global using Microsoft.EntityFrameworkCore; // Untuk DbContext mocks
global using Moq;