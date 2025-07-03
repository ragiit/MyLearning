namespace Apple.Services.CouponAPI.Models.Dto
{
    // DTO ini berfungsi sebagai pembungkus standar untuk semua respons API.
    // Ini membantu client untuk memproses respons dengan cara yang konsisten.
    public class ResponseDto
    {
        // Menunjukkan apakah permintaan berhasil atau tidak.
        public bool IsSuccess { get; set; } = true;

        // Berisi pesan kesalahan atau informasi.
        public string Message { get; set; } = string.Empty;

        // Berisi data hasil jika permintaan berhasil.
        public object? Result { get; set; }
    }
}