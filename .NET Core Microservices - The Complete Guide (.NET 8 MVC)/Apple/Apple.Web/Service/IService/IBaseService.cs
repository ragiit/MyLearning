namespace Apple.Web.Service.IService
{
    public interface IBaseService
    {
        // Metode generik untuk mengirim permintaan ke API dan menerima respons.
        Task<ResponseDto?> SendAsync(RequestDto requestDto, bool isWithToken = true);
    }
}