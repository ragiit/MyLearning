namespace Menu.API.Services.IService
{
    public interface IFileStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);

        Task DeleteFileAsync(string fileName, string folderName);
    }
}