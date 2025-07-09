# MyApp - ASP.NET 8 N-Layered API

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![Architecture](https://img.shields.io/badge/Architecture-N--Layered-informational)
![Status](https://img.shields.io/badge/Status-Example-green)
![License](https://img.shields.io/badge/License-MIT-blue)

Proyek ini adalah contoh aplikasi Web API yang dibangun menggunakan **ASP.NET 8** dengan menerapkan **Layered Architecture (N-Tier)**. Aplikasi ini mendemonstrasikan pemisahan tanggung jawab yang jelas dan penggunaan pola desain modern seperti *Repository*, *Unit of Work*, dan *DTO* dengan pemetaan objek menggunakan *Mapster*.

## Arsitektur yang Digunakan: Layered Architecture (N-Tier)

Prinsip utama arsitektur ini adalah memisahkan aplikasi menjadi lapisan-lapisan logis yang independen. Setiap lapisan memiliki tanggung jawab spesifik dan hanya berkomunikasi dengan lapisan yang berdekatan dengannya.

### Diagram Ketergantungan (Dependency Flow)

+--------------------------------+
|       Klien (Browser/App)      |
+--------------------------------+
| (HTTP Request)
V
+--------------------------------+
|    Presentation Layer (.Api)   |  (Controllers, API Response Wrapper)
+--------------------------------+
| (Memanggil Service)
V
+--------------------------------+
|  Business Logic Layer (.Service) |  (Services, DTOs, Business Logic, Mapster)
+--------------------------------+
| (Memanggil Unit of Work)
V
+--------------------------------+
|    Data Access Layer (.Data)   |  (DbContext, Repositories, Unit of Work)
+--------------------------------+
| (Mengimplementasikan Interface)
V
+--------------------------------+
|         Core Layer (.Core)     |  (Entities, Repository Interfaces)
+--------------------------------+
| (Berinteraksi via EF Core)
V
+--------------------------------+
|           Database             |  (SQL Server)
+--------------------------------+

### Alur Kerja Aplikasi
1.  **Request Masuk**: Klien mengirim request HTTP yang diterima oleh `AuthorsController` di **Presentation Layer** (`MyApp.Api`).
2.  **Delegasi ke Service**: Controller memvalidasi request dan memanggil method yang sesuai di `IAuthorService` di **Business Logic Layer** (`MyApp.Service`).
3.  **Eksekusi Logika Bisnis**: `AuthorService` menjalankan logika bisnis (jika ada). Untuk operasi penulisan data, ia menggunakan Mapster untuk mengubah DTO menjadi entitas domain.
4.  **Akses Data terkoordinasi**: `AuthorService` memanggil `IUnitOfWork` untuk mengakses repositori yang dibutuhkan, misalnya `_unitOfWork.Authors`.
5.  **Interaksi Database**: `AuthorRepository` di **Data Access Layer** (`MyApp.Data`) menggunakan Entity Framework Core `AppDbContext` untuk menerjemahkan panggilan method menjadi query SQL ke database. Semua operasi (misalnya beberapa `Add` dan `Update`) dibungkus dalam satu transaksi oleh `UnitOfWork`.
6.  **Alur Kembali**: Hasil dari database dikembalikan ke atas. Di `Service Layer`, entitas di-mapping kembali ke DTO. Di `Api Layer`, DTO dibungkus dalam `ApiResponse<T>` dan dikirim sebagai response HTTP ke klien.

## Struktur Proyek
MyApp/
│
├── MyApp.sln
│
├── MyApp.Api/                (👈 PRESENTATION LAYER)
│   ├── Common/
│   │   └── ApiResponse.cs
│   ├── Controllers/
│   │   └── AuthorsController.cs
│   ├── appsettings.json
│   └── Program.cs
│
├── MyApp.Service/            (👈 BUSINESS LOGIC LAYER)
│   ├── DTOs/
│   │   ├── AuthorDto.cs
│   │   └── CreateAuthorDto.cs
│   ├── Interfaces/
│   │   └── IAuthorService.cs
│   └── Services/
│       └── AuthorService.cs
│
├── MyApp.Data/               (👈 DATA ACCESS LAYER)
│   ├── Repositories/
│   │   └── AuthorRepository.cs
│   ├── AppDbContext.cs
│   └── UnitOfWork.cs
│
└── MyApp.Core/               (👈 CORE / DOMAIN LAYER)
├── Entities/
│   ├── Author.cs
│   └── Book.cs
└── Interfaces/
├── IAuthorRepository.cs
├── IGenericRepository.cs
└── IUnitOfWork.cs


## Teknologi yang Digunakan
- **Framework**: .NET 8, ASP.NET Core Web API
- **ORM**: Entity Framework Core
- **Database**: SQL Server
- **Arsitektur & Pola**: N-Tier/Layered, Repository Pattern, Unit of Work Pattern
- **Lainnya**: Mapster (Object-to-Object Mapping), DTO (Data Transfer Object)

## Cara Menjalankan Proyek

### Prasyarat
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Edisi Express, Developer, atau lainnya)

### Langkah-langkah
1.  **Clone repository ini:**
    ```bash
    git clone [https://github.com/NAMA_USER_ANDA/NAMA_REPO_ANDA.git](https://github.com/NAMA_USER_ANDA/NAMA_REPO_ANDA.git)
    cd NAMA_REPO_ANDA
    ```
2.  **Konfigurasi Database:**
    Buka file `MyApp.Api/appsettings.json` dan ubah `ConnectionStrings` agar sesuai dengan konfigurasi SQL Server Anda.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=NAMA_SERVER_ANDA;Database=MyAppDb;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```

3.  **Terapkan Migrasi Database:**
    Buka terminal di root folder proyek dan jalankan perintah berikut untuk membuat database dan tabelnya.
    ```bash
    dotnet ef database update --project MyApp.Data -s MyApp.Api
    ```

4.  **Jalankan aplikasi:**
    ```bash
    dotnet run --project MyApp.Api
    ```
5.  **Buka Dokumentasi API:**
    Buka browser Anda dan navigasi ke `https://localhost:xxxx/swagger` (ganti `xxxx` dengan port yang benar) untuk melihat dokumentasi API dan melakukan pengujian.

## Dokumentasi API (Endpoints)

Semua endpoint mengembalikan data dalam format `ApiResponse<T>`.

| Method | Endpoint             | Deskripsi                               |
|--------|----------------------|-----------------------------------------|
| `GET`  | `/api/authors`       | Mengambil semua data penulis.           |
| `GET`  | `/api/authors/{id}`  | Mengambil data penulis berdasarkan ID.  |
| `POST` | `/api/authors`       | Membuat penulis baru.                   |

## Lisensi
Proyek ini dilisensikan di bawah Lisensi MIT. Lihat file `LICENSE` untuk detailnya.