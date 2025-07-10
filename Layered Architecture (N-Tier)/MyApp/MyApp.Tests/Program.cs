using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

// normal config...

var app = builder.Build();

// pipeline...

app.Run();

// WAJIB:

public partial class Program
{ } // <<< supaya test bisa instantiate host