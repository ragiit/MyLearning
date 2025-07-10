# MyApp — ASP.NET Core 8 Web API — N-Tier Architecture

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Architecture](https://img.shields.io/badge/Architecture-N--Tier-informational)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 📌 Overview

MyApp adalah contoh **ASP.NET Core Web API** dengan arsitektur **N-Tier** yang clean, modular & production-ready.

Fitur utama:
- ✅ N-Tier Architecture (`Api` ➜ `Business` ➜ `Data` ➜ `Domain`)
- ✅ Entity Framework Core + SQL Server
- ✅ Async, `AsNoTracking`, Pagination
- ✅ DTO Mapping pakai **Mapster**
- ✅ Validation pakai **FluentValidation**
- ✅ Logging pakai **Serilog**
- ✅ Custom Middleware Logging Request & Timer
- ✅ Global Exception Handler Middleware (`BaseResponse.Fail`)
- ✅ Swagger UI
- ✅ Health Checks + UI Monitoring
- ✅ Rate Limiting & Response Caching
- ✅ Automatic DB Migration on Startup

---

## ⚙️ Tech Stack

- **ASP.NET Core 8**
- **EF Core 8**
- **SQL Server**
- **Mapster**
- **FluentValidation**
- **Serilog**
- **HealthChecks.UI**
- **Swagger**

---

## 📂 Project Structure

```plaintext
MyApp/
│
├── MyApp.Api/ # Presentation Layer (Controllers, Middleware)
├── MyApp.Business/ # Business Logic Layer (Services, Validators)
├── MyApp.Data/ # Data Access Layer (DbContext, Repositories)
├── MyApp.Domain/ # Domain Layer (Entities, DTOs, BaseResponse)
├── MyApp.sln
└── README.md
```

---

| Method  | Endpoint             | Description                     |
| ------- | -------------------- | ------------------------------- |
| GET     | `/api/products`      | Get all products (AsNoTracking) |
| GET     | `/api/products/{id}` | Get product by ID               |
| POST    | `/api/products`      | Create new product              |
| PUT     | `/api/products`      | Update product                  |
| DELETE  | `/api/products/{id}` | Delete product by ID            |
| GET     | `/health`            | Health check JSON status        |
| GET     | `/healthchecks-ui`   | Health check dashboard UI       |
| Swagger | `/swagger`           | API documentation UI            |

🧩 Key Concepts
✅ Middleware
Request Logging Middleware

Log setiap request + durasi pakai Stopwatch + Serilog.

Error Handling Middleware

Tangkap semua unhandled exception.

Log error ke file.

Return response BaseResponse.Fail(...) (tanpa stack trace).

✅ Pagination & Async
Semua repo/service pakai AsNoTracking()

Paged query Skip + Take + Count.

✅ DTO Mapping
Mapping Entity ⟷ DTO pakai Mapster.

Register TypeAdapterConfig di AddMapster().

✅ Validation
FluentValidation.

Validator di Business/Validators/ProductValidator.cs.

✅ Logging
Serilog:

Write to Console & Rolling File Logs/log-<date>.txt

Konfigurasi di Program.cs.

✅ Health Check
AddHealthChecks() + AddDbContextCheck()

Custom HealthCheck (contoh: disk space).

UI Monitoring dengan AspNetCore.HealthChecks.UI + InMemory Storage.

✅ Rate Limiting & Compression
AddRateLimiter (Fixed Window)

Response Caching

Response Compression (Brotli + Gzip)