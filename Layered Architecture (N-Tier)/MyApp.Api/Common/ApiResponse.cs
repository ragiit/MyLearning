using System.Text.Json.Serialization;

namespace MyApp.Api.Common
{
    // Gunakan T untuk membuatnya generik, bisa untuk tipe data apa saja
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; private set; }
        public int Code { get; private set; }
        public string Message { get; private set; }

        // Data bisa null, misalnya untuk response gagal
        public T? Data { get; private set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<string>? Errors { get; private set; }

        // Constructor privat agar pembuatan objek terkontrol lewat static method
        private ApiResponse()
        { }

        // --- Static Factory Methods untuk membuat instance ApiResponse ---

        public static ApiResponse<T> Success(T data, string message = "Request successful.", int code = 200)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Code = code,
                Message = message,
                Data = data
            };
        }

        // Overload untuk sukses tanpa data (misal: untuk request DELETE)
        public static ApiResponse<object> Success(string message = "Request successful.", int code = 200)
        {
            return new ApiResponse<object>
            {
                IsSuccess = true,
                Code = code,
                Message = message,
                Data = null
            };
        }

        public static ApiResponse<T> Fail(string message, int code = 400, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Code = code,
                Message = message,
                Errors = errors
            };
        }
    }
}