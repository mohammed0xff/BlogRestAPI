using Microsoft.AspNetCore.Http;

namespace Services.Storage
{
    public interface IStorageService
    {
        public string UploadProfileImage(IFormFile ImageFile, string username);
        public void DeleteProfileImage(string username);
    }
}
