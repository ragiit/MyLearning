using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Identity.API.Persistence.Entities;

/// <summary>
/// Representasi pengguna dalam sistem AegisMedical.
/// Model ini mencakup semua jenis pengguna, dari Pasien hingga Staf Medis dan Manajemen.
/// </summary>
public class ApplicationUser : IdentityUser
{
    // =================================================================
    // SECTION: DATA PRIBADI UMUM (Berlaku untuk semua pengguna)
    // =================================================================

    /// <summary>
    /// Nama lengkap pengguna sesuai KTP/identitas resmi.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Gelar profesional (contoh: dr., Prof., S.Kep., Ns., Apt.).
    /// </summary>
    [MaxLength(50)]
    public string? Title { get; set; }

    /// <summary>
    /// Tanggal lahir pengguna.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Jenis kelamin pengguna (contoh: "Laki-laki", "Perempuan").
    /// </summary>
    [MaxLength(20)]
    public string? Gender { get; set; }

    /// <summary>
    /// Alamat lengkap tempat tinggal pengguna.
    /// </summary>
    [MaxLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// URL foto profil pengguna.
    /// </summary>
    [MaxLength(1024)]
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Status aktif atau non-aktif pengguna di dalam sistem.
    /// </summary>
    public bool IsActive { get; set; } = true;

    // =================================================================
    // SECTION: DATA KHUSUS PASIEN
    // =================================================================

    /// <summary>
    /// Nomor Rekam Medis (NRM) unik untuk setiap pasien.
    /// Hanya diisi jika pengguna memiliki peran "Pasien".
    /// </summary>
    [MaxLength(50)]
    public string? MedicalRecordNumber { get; set; }

    /// <summary>
    /// Golongan darah pasien (A, B, AB, O).
    /// </summary>
    [MaxLength(2)]
    public string? BloodType { get; set; }

    /// <summary>
    /// Catatan riwayat alergi yang dimiliki pasien.
    /// </summary>
    public string? AllergyHistory { get; set; }

    /// <summary>
    /// Nama wali atau kontak darurat pasien.
    /// </summary>
    [MaxLength(200)]
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Nomor telepon kontak darurat pasien.
    /// </summary>
    [MaxLength(20)]
    public string? EmergencyContactPhone { get; set; }

    // =================================================================
    // SECTION: DATA KHUSUS STAF MEDIS & KEPEGAWAIAN
    // =================================================================

    /// <summary>
    /// Nomor Induk Pegawai (NIP) atau ID Karyawan.
    /// Berlaku untuk semua staf rumah sakit.
    /// </summary>
    [MaxLength(50)]
    public string? EmployeeId { get; set; }

    /// <summary>
    /// Tanggal pertama kali staf bergabung dengan rumah sakit.
    /// </summary>
    public DateTime? HireDate { get; set; }

    /// <summary>
    /// Spesialisasi utama, relevan untuk Dokter, Perawat, dll.
    /// Contoh: "Spesialis Jantung dan Pembuluh Darah", "Perawat IGD".
    /// </summary>
    [MaxLength(150)]
    public string? Specialization { get; set; }

    /// <summary>
    /// Nomor Surat Tanda Registrasi (STR) yang dikeluarkan oleh konsil kedokteran/kesehatan.
    /// Wajib untuk Dokter, Perawat, Apoteker.
    /// </summary>
    [MaxLength(100)]
    public string? LicenseNumberSTR { get; set; }

    /// <summary>
    /// Nomor Surat Izin Praktik (SIP) yang berlaku.
    /// Wajib untuk Dokter dan praktisi medis lainnya.
    /// </summary>
    [MaxLength(100)]
    public string? LicenseNumberSIP { get; set; }

    /// <summary>
    /// Catatan tambahan mengenai kualifikasi atau sertifikasi lain yang dimiliki staf.
    /// </summary>
    public string? Qualifications { get; set; }
}