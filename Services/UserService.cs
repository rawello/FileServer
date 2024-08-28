using FileServerApp.Data;
using FileServerApp.Models;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace FileServerApp.Services
{
    public class UserService : IUserService
    {
        private readonly FileServerDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(FileServerDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public User Authenticate(string username, string password)
        {
            _logger.LogInformation($"Authenticating user: {username}");
            return _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public bool Register(User user)
        {
            _logger.LogInformation($"Registering user: {user.Username}");
            if (_dbContext.Users.Any(u => u.Username == user.Username))
            {
                _logger.LogWarning($"Username already exists: {user.Username}");
                return false;
            }
            user.Role = user.Role ?? "User"; 
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
            _logger.LogInformation($"User registered successfully: {user.Username}");
            return true;
        }
    }
}