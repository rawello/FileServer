using FileServerApp.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileServerApp.Services
{
    public interface IFileService
    {
        Task<byte[]> GetFileAsync(string filename, int userId, string userRole);
        Task<bool> UploadFileAsync(IFormFile file, int userId);
        Task<bool> DeleteFileAsync(string filename, int userId, string userRole);
        Task<IEnumerable<FileModel>> GetUserFilesAsync(int userId);
    }
}