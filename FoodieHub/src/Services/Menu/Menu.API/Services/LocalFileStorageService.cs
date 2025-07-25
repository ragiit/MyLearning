using Menu.API.Services.IService;

namespace Menu.API.Services
{
    public class LocalFileStorageService(IHostEnvironment env) : IFileStorageService
    {
        public async Task<string> UploadFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            var uploadsFolder = Path.Combine(env.ContentRootPath, "wwwroot", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return URL atau path relatif
            return $"/{folderName}/{uniqueFileName}";
        }

        public Task DeleteFileAsync(string fileName, string folderName)
        {
            var filePath = Path.Combine(env.ContentRootPath, "wwwroot", folderName, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return Task.CompletedTask;
        }
    }
}