global using BuildingBlocks.Behaviors;
global using BuildingBlocks.CQRS;
global using BuildingBlocks.Exceptions.Handler;
global using BuildingBlocks.Helper;
global using BuildingBlocks.Pagination;
global using FluentValidation;
global using Mapster;
global using MediatR;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.RateLimiting;
global using Microsoft.AspNetCore.ResponseCompression;
global using Basket.API.Configuration;
global using Microsoft.EntityFrameworkCore;

global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using System.ComponentModel.DataAnnotations;
global using System.IO.Compression;
global using System.Text;
global using static BuildingBlocks.Helper.Helper;
global using BuildingBlocks.Exceptions;
global using Basket.API.Dtos;
global using Basket.API.Data;
global using Basket.API.Exceptions;
global using Basket.API.Services;