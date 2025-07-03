namespace Apple.Web.Service.IService
{
    public interface IAuthService
    {
        // Mengirim permintaan login ke API
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);

        // Mengirim permintaan registrasi ke API
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);

        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto assignRoleRequestDto);

        // Melakukan proses logout di sisi client
        Task LogoutAsync();
    }
}