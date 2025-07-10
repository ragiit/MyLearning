using System.ComponentModel.DataAnnotations;

namespace Apple.Web.Utility
{
    public class AllowedExtensionsAttribute(string[] extensions, int maxFileSizeMB = 5) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Cek jika value adalah IFormFile
            if (value is IFormFile file)
            {
                // 1. Validasi Ekstensi File
                var extension = Path.GetExtension(file.FileName);
                if (!extensions.Contains(extension.ToLower()))
                {
                    // Jika ekstensi tidak diizinkan, kembalikan pesan error.
                    return new ValidationResult($"This file extension is not allowed. Allowed extensions are: {string.Join(", ", extensions)}");
                }

                // 2. Validasi Ukuran File
                if (file.Length > (maxFileSizeMB * 1024 * 1024))
                {
                    return new ValidationResult($"The file size exceeds the maximum limit of {maxFileSizeMB} MB.");
                }
            }
            // Jika validasi berhasil atau value bukan IFormFile, kembalikan sukses.
            return ValidationResult.Success;
        }
    }
}