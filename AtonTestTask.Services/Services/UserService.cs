using AtonTestTask.Core;
using AtonTestTask.Core.Models;
using AtonTestTask.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AtonTestTask.Services
{
    public class UserService: IUserService
    {
        private readonly ApplicationContext _db;

        public UserService(ApplicationContext db)
        {
            _db = db;
            if(_db.Users.FirstOrDefault(u => u.Login == "AdminTest" && u.Password == "123123") is null)
                InitilizeAdmin(_db);
        }

        private static void InitilizeAdmin(ApplicationContext _db)
        {
            User admin = new User
            {
                Login = "AdminTest",
                Password = "123123",
                Name = "DevName",
                Gender = 2,
                Admin = true,
                CreatedOn = DateTime.Now,
                CreatedBy = "AdminTest",
            };
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Add(admin);
                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                }
            }
        }

        public User? GetByLogin(string? login)
            => _db.Users.FirstOrDefault(u => u.Login == login);

        public User? GetByLoginPassword(string login, string password)
            => _db.Users.FirstOrDefault(u => u.Login == login && u.Password == password);

        public User? Update(string login, User user, string? oldLogin = "")
        {
            User? curUser = GetByLogin(Equals(oldLogin, "") ? user.Login : oldLogin!);
            if (curUser is null)
                return null;
            curUser.Copy(user);
            curUser.ModifiedOn = DateTime.Now;
            curUser.ModifiedBy = login;
            _db.SaveChanges();
            return curUser;
        }

        public async Task<List<User>?> GetAll(bool active = false)
        {
            if (active)
                return await _db.Users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn).ToListAsync();
            return await _db.Users.OrderBy(u => u.CreatedOn).ToListAsync();
        }

        public IEnumerable<User>? GetUsersOlderAge(int age)
            => _db.Users.ToList().Where(u => u.GetAge() >= age);

        public User? Add(User user)
        {
            User? tmpUser = GetByLogin(user.Login);
            if(tmpUser is not null) { return null; }
            using(var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    _db.Users.Add(user);
                    _db.SaveChanges();
                    transaction.Commit();
                    return user;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }
        }

        public (bool IsSuccess, bool IsFound) Delete(string adminLogin, string userLogin, bool isSoft)
        {
            User? user = GetByLogin(userLogin);
            if (user is null)
                return (false, false);

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    if (isSoft) {
                        user.RevokedOn = DateTime.Now;
                        user.RevokedBy = adminLogin;
                    }
                    else
                        _db.Users.Remove(user);

                    _db.SaveChanges();
                    transaction.Commit();
                    return (true, true);
                }
                catch
                {
                    transaction.Rollback();
                    return (false, true);
                }
            }
        }

        public bool Recovery(string login)
        {
            User? user = GetByLogin(login);
            if (user is null)
                return false;

            user.RevokedOn = null;
            user.RevokedBy = null;

            _db.SaveChanges();
            return true;
        }
    }
}
