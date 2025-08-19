namespace Identity.API.Persistence.Seed;

/// <summary>
/// Menyediakan data pengguna default untuk di-seed ke dalam database.
/// </summary>
public static class IdentityUsers
{
    /// <summary>
    /// Daftar pengguna default, masing-masing dengan peran, email, dan nama lengkap yang telah ditentukan.
    /// </summary>
    public static readonly (string Role, string Email, string FullName)[] DefaultUsers =
    [
        (IdentityRoles.SuperAdmin, "sysadmin@aegismedical.com", "System Administrator"),
        (IdentityRoles.Manajemen, "manajemen@aegismedical.com", "Kepala Manajemen RS"),
        (IdentityRoles.StafPendaftaran, "reg.staff@aegismedical.com", "Budi Santoso (Pendaftaran)"),
        (IdentityRoles.StafKeuangan, "finance.staff@aegismedical.com", "Dewi Lestari (Keuangan)"),
        (IdentityRoles.Dokter, "dr.mahardika@aegismedical.com", "dr. Mahardika Putra"),
        (IdentityRoles.Perawat, "nurse.susan@aegismedical.com", "Susan Amelia, A.Md.Kep"),
        (IdentityRoles.Apoteker, "apt.rahmat@aegismedical.com", "Rahmat Hidayat, S.Farm., Apt."),
        (IdentityRoles.Laboran, "lab.indah@aegismedical.com", "Indah Permata, A.Md.AK"),
        (IdentityRoles.Pasien, "pasien.demo@aegismedical.com", "John Doe (Pasien)")
    ];
}