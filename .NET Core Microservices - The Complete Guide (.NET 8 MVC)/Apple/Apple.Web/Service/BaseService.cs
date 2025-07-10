using System.Net.Http.Headers;

namespace Apple.Web.Service
{
    public class BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider) : IBaseService
    {
        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool isWithToken = true)
        {
            try
            {
                // Membuat HttpClient dari factory.
                HttpClient client = httpClientFactory.CreateClient("AppleAPI");
                HttpRequestMessage message = new();

                // Mengatur header permintaan.
                message.Headers.Add("Accept", "application/json");
                // TODO: Tambahkan token jika diperlukan di masa depan.
                // message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", requestDto.AccessToken);
                if (isWithToken)
                {
                    var token = tokenProvider.GetToken();
                    if (!string.IsNullOrEmpty(token))
                    {
                        message.Headers.Add("Authorization", $"Bearer {token}");
                    }
                }
                // Mengatur URL dan metode HTTP.
                message.RequestUri = new Uri(requestDto.Url);

                // Jika ada data (untuk POST/PUT), serialisasi ke JSON.
                if (requestDto.Data != null)
                {
                    if (requestDto.Data is ProductDto productDto && productDto.Image != null)
                    {
                        // Buat FormData
                        var formData = new MultipartFormDataContent();

                        formData.Add(new StringContent(productDto.Id.ToString()), nameof(productDto.Id));
                        formData.Add(new StringContent(productDto.Name), nameof(productDto.Name));
                        formData.Add(new StringContent(productDto.Price.ToString()), nameof(productDto.Price));
                        formData.Add(new StringContent(productDto.Description ?? ""), nameof(productDto.Description));
                        formData.Add(new StringContent(productDto.CategoryName ?? ""), nameof(productDto.CategoryName));
                        formData.Add(new StringContent(productDto.Count.ToString()), nameof(productDto.Count));

                        var imageContent = new StreamContent(productDto.Image.OpenReadStream());
                        imageContent.Headers.ContentType = new MediaTypeHeaderValue(productDto.Image.ContentType);
                        formData.Add(imageContent, nameof(productDto.Image), productDto.Image.FileName);

                        message.Content = formData;
                    }
                    else
                    {
                        // Default ke JSON
                        message.Content = new StringContent(
                            JsonConvert.SerializeObject(requestDto.Data),
                            Encoding.UTF8,
                            "application/json"
                        );
                    }
                }

                // Mengatur metode HTTP berdasarkan ApiType.
                message.Method = requestDto.ApiType switch
                {
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };

                // Mengirim permintaan ke API.
                HttpResponseMessage? apiResponse = await client.SendAsync(message);

                // Menangani respons berdasarkan status code.
                switch (apiResponse.StatusCode)
                {
                    case System.Net.HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };

                    case System.Net.HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };

                    case System.Net.HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };

                    case System.Net.HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };

                    default:
                        // Jika sukses, baca konten respons dan deserialisasi.
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                // Menangani kesalahan jika terjadi pengecualian.
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}