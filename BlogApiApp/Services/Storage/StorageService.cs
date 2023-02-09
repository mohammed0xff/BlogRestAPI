using Microsoft.AspNetCore.Http;

namespace Services.Storage
{
    public class StorageService : IStorageService
    {
        public string ProfileRootPath = "./Resources/Images/Profile";
        public void DeleteProfileImage(string username)
        {
            var imagePath = Path.Combine(ProfileRootPath, username);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }

        public string UploadProfileImage(IFormFile ImageFile, string username)
        {
            var imagePath = Path.Combine(ProfileRootPath, username);
            using (var stream = File.Create(imagePath))
            {
                ImageFile.CopyTo(stream);
            }
            return imagePath;
        }
    }
}
