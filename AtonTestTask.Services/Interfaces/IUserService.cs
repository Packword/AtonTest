using AtonTestTask.Core.Models;
using System.Security.Claims;

namespace AtonTestTask.Services.Interfaces
{
    public interface IUserService
    {
        public User? GetByLogin(string? login);
        public User? GetByLoginPassword(string login, string password);
        public User? Update(string login, User user, string oldLogin = "");
        public Task<List<User>?> GetAll(bool active = false);
        public IEnumerable<User>? GetUsersOlderAge(int age);
        public User? Add(User user);
        public (bool IsSuccess, bool IsFound) Delete(string adminLogin, string userLogin, bool isSoft);
        public bool Recovery(string login);
    }
}
