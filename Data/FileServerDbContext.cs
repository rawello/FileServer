using FileServerApp.Models;
using Microsoft.EntityFrameworkCore;

namespace FileServerApp.Data
{
    public class FileServerDbContext : DbContext
    {
        public FileServerDbContext(DbContextOptions<FileServerDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<FileModel> Files { get; set; }
    }
}