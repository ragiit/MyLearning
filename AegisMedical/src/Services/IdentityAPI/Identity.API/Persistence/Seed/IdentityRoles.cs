namespace Identity.API.Persistence.Seed;

/// <summary>
/// Mendefinisikan konstanta untuk peran-peran utama dalam aplikasi AegisMedical.
/// </summary>
public static class IdentityRoles
{
    // --- Peran Level Tertinggi ---
    public const string SuperAdmin = "SuperAdmin"; // Memiliki akses penuh ke seluruh sistem, untuk developer atau IT utama.

    // --- Peran Manajemen & Administrasi ---
    public const string Manajemen = "Manajemen";     // Untuk direksi atau kepala bagian, melihat laporan dan statistik.

    public const string StafPendaftaran = "StafPendaftaran"; // Mendaftarkan pasien baru, membuat janji temu.
    public const string StafKeuangan = "StafKeuangan";   // Mengelola tagihan, pembayaran, dan asuransi.

    // --- Peran Klinis / Medis ---
    public const string Dokter = "Dokter";         // Mengelola rekam medis, memberikan diagnosis, meresepkan obat.

    public const string Perawat = "Perawat";        // Melakukan tindakan keperawatan, mencatat vital sign.
    public const string Apoteker = "Apoteker";      // Mengelola stok obat, memvalidasi dan menyerahkan resep.
    public const string Laboran = "Laboran";        // Mengelola hasil tes laboratorium.

    // --- Peran Pasien ---
    public const string Pasien = "Pasien";          // Mengakses data rekam medis pribadi, melihat jadwal janji temu.

    /// <summary>
    /// Daftar semua peran default yang akan di-seed ke database saat aplikasi pertama kali berjalan.
    /// </summary>
    public static readonly string[] DefaultRoles =
    [
        SuperAdmin,
        Manajemen,
        StafPendaftaran,
        StafKeuangan,
        Dokter,
        Perawat,
        Apoteker,
        Laboran,
        Pasien
    ];
}