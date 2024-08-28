using FileServerApp.Data;
using FileServerApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileServerApp.Services
{
    public class FileService : IFileService
    {
        private readonly FileServerDbContext _dbContext;
        private readonly string _filesPath;

        public FileService(FileServerDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _filesPath = configuration["FileStoragePath"];
        }

        public async Task<byte[]> GetFileAsync(string filename, int userId, string userRole)
        {
            var file = await _dbContext.Files
                .FirstOrDefaultAsync(f => f.FileName == filename && (f.UserId == userId || userRole == "Admin"));

            if (file != null)
            {
                var filePath = Path.Combine(_filesPath, file.FilePath);
                if (File.Exists(filePath))
                {
                    return await File.ReadAllBytesAsync(filePath);
                }
            }
            return null;
        }

        public async Task<bool> UploadFileAsync(IFormFile file, int userId)
        {
            var userFolderPath = Path.Combine(_filesPath, userId.ToString());
            if (!Directory.Exists(userFolderPath))
            {
                Directory.CreateDirectory(userFolderPath);
            }

            var filePath = Path.Combine(userFolderPath, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _dbContext.Files.Add(new FileModel
            {
                FileName = file.FileName,
                FilePath = Path.Combine(userId.ToString(), file.FileName),
                UserId = userId
            });
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFileAsync(string filename, int userId, string userRole)
        {
            var file = await _dbContext.Files
                .FirstOrDefaultAsync(f => f.FileName == filename && (f.UserId == userId || userRole == "Admin"));

            if (file != null)
            {
                var filePath = Path.Combine(_filesPath, file.FilePath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                _dbContext.Files.Remove(file);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<FileModel>> GetUserFilesAsync(int userId)
        {
            return await _dbContext.Files
                .Where(f => f.UserId == userId)
                .Include(f => f.User)
                .ToListAsync();
        }
    }
}