using FileServerApp.Models;

namespace FileServerApp.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        bool Register(User user);
    }
}