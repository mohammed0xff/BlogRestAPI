using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Storage
{
    public interface IStorageService
    {
        public string UploadProfileImage(IFormFile ImageFile, string username);
        public void DeleteProfileImage(string username);
    }
}
