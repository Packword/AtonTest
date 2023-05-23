using AtonTestTask.Core.Models;
using AtonTestTask.Services.Interfaces;
using AtonTestTask.Web.DTOModels;
using AtonTestTask.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AtonTestTask.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Создание пользователя
        /// </summary>
        /// <param name="userDto">Пользователь</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(CreateUserDTO userDto)
        {
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return Unauthorized("Incorrect request user data");

            if (!IsAdmin(reqUser!))
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied!");

            User? res = _userService.Add(new User
            {
                Login = userDto.Login,
                Password = userDto.Password,
                Name = userDto.Name,
                Gender = userDto.Gender,
                Birthday = userDto.Birthday,
                Admin = userDto.Admin,
                CreatedOn = DateTime.Now,
                CreatedBy = reqUser!.Login
            });
            if (res is null)
                return BadRequest("Incorrect arguments or dublicate");

            return StatusCode(StatusCodes.Status201Created, res);
        }

        /// <summary>
        /// Изменение имени, гендера или даты рождения пользователя
        /// </summary>
        /// <param name="userDto">Пользователь</param>
        /// <param name="userLogin">Логин изменяемого пользователя</param>
        /// <returns></returns>
        [HttpPut("info/{userLogin}")]
        public IActionResult UpdateInfo(UpdateUserInfoDTO userDto, string userLogin)
        {
            var validation = ValidateUpdateDTOAndCheckAccess(userDto, userLogin);
            if (!validation.Res)
                return StatusCode(validation.StatusCode, validation.Msg);
            User reqUser = validation.ReqUser!;
            User updateUser = validation.UpdateUser!;

            updateUser.Name = userDto.Name ?? updateUser.Name;
            updateUser.Gender = userDto.Gender ?? updateUser.Gender;
            updateUser.Birthday = userDto.Birthday ?? updateUser.Birthday;

            var res = _userService.Update(reqUser.Login, updateUser);
            if (res is null)
                return BadRequest("Info updating error");
            return Ok();
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="userLogin">Логин изменяемого пользователя</param>
        /// <param name="userDto">Пользователь</param>
        /// <returns></returns>
        [HttpPut("password/{userLogin}")]
        public IActionResult UpdatePassword(UpdateUserPasswordDTO userDto, string userLogin)
        {
            var validation = ValidateUpdateDTOAndCheckAccess(userDto, userLogin);
            if (!validation.Res)
                return StatusCode(validation.StatusCode, validation.Msg);
            User reqUser = validation.ReqUser!;
            User updateUser = validation.UpdateUser!;

            updateUser.Password = userDto.Password;

            var res = _userService.Update(reqUser.Login, updateUser);
            if (res is null)
                return BadRequest("Password updating error");
            return Ok();
        }

        /// <summary>
        /// Изменение логина пользователя
        /// </summary>
        /// <param name="userLogin">Логин изменяемого пользователя</param>
        /// <param name="userDto">Пользователь</param>
        /// <returns></returns>
        [HttpPut("login/{userLogin}")]
        public IActionResult UpdateLogin(UpdateUserLoginDTO userDto, string userLogin)
        {
            var validation = ValidateUpdateDTOAndCheckAccess(userDto, userLogin);
            if (!validation.Res)
                return StatusCode(validation.StatusCode, validation.Msg);
            User reqUser = validation.ReqUser!;
            User updateUser = validation.UpdateUser!;

            User? uniqueCheck = _userService.GetByLogin(userDto.NewLogin);
            if (uniqueCheck is not null)
                return BadRequest("Dublicate login");

            string oldLogin = updateUser.Login;
            updateUser.Login = userDto.NewLogin;

            var res = _userService.Update(reqUser.Login, updateUser, oldLogin);
            if (res is null)
                return BadRequest("Login updating error");
            return Ok();
        }

        /// <summary>
        /// Восстановление пользователя
        /// </summary>
        /// <param name="userDto">Автор запроса</param>
        /// <param name="userLogin">Логин пользователя</param>
        /// <returns></returns>
        [HttpPut("recovery/{userLogin}")]
        public IActionResult Recovery(BaseUserDTO userDto, string userLogin)
        {
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return Unauthorized("Incorrect request user data");

            if (!IsAdmin(reqUser!))
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied!");

            bool recoveryResult = _userService.Recovery(userLogin);
            if (recoveryResult == false)
                return NotFound();
            else
                return Ok();
        }

        /// <summary>
        /// Получение всех активных пользователей
        /// </summary>
        /// <param name="login">Логин автора запроса</param>
        /// <param name="password">Пароль автора запроса</param>
        /// <returns></returns>
        [HttpGet("active/")]
        public async Task<IActionResult> GetActive(string login, string password)
        {
            BaseUserDTO userDto = new BaseUserDTO { RequestUser = new RequestUserDTO { Login = login, Password = password } };
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return Unauthorized("Incorrect request user data");

            if (!IsAdmin(reqUser!))
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied!");

            return Ok(await _userService.GetAll(true));
        }

        /// <summary>
        /// Получение информации о пользователе по его логину
        /// </summary>
        /// <param name="login">Логин автора запроса</param>
        /// <param name="password">Пароль автора запроса</param>
        /// <param name="userLogin">Логин искомого пользователя</param>
        /// <returns></returns>
        [HttpGet("{userLogin}")]
        public IActionResult GetUserByLogin(string login, string password, string userLogin)
        {
            BaseUserDTO userDto = new BaseUserDTO { RequestUser = new RequestUserDTO { Login = login, Password = password } };
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return Unauthorized("Incorrect request user data");

            if (!IsAdmin(reqUser!))
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied!");

            User? user = _userService.GetByLogin(userLogin);
            if (user is null)
                return NotFound();
            return Ok(new UserInfoViewModel
            {
                Name = user.Name,
                Gender = user.Gender switch
                {
                    0 => "Female",
                    1 => "Male",
                    2 => "Uknown",
                    _ => "Error"
                },
                Birthday = user.Birthday,
                IsActive = user.RevokedOn is null ? true : false
            });
        }

        /// <summary>
        /// Получение информации о пользователе по логину и паролю
        /// </summary>
        /// <param name="login">Логин автора запроса</param>
        /// <param name="password">Пароль автора запроса</param>
        /// <param name="userLogin">Логин искомого пользователя</param>
        /// <param name="userPassword">Пароль искомого пользователя</param>
        /// <returns></returns>
        [HttpGet("{userLogin}/{userPassword}")]
        public IActionResult GetUser(string login, string password, string userLogin, string userPassword)
        {
            BaseUserDTO userDto = new BaseUserDTO { RequestUser = new RequestUserDTO { Login = login, Password = password } };
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return Unauthorized("Incorrect request user data");      // the reqUser and the user we are looking for are identical in this case,
                                                                         // but we will still search for the user again, since a token or cookies may be used here in the future
            if (!IsUserExistsAndActive(userLogin, reqUser!))
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied!");

            User? user = _userService.GetByLoginPassword(userLogin, userPassword);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Получение всех пользователей старше определённого возраста
        /// </summary>
        /// <param name="login">Логин автора запроса</param>
        /// <param name="password">Пароль автора запроса</param>
        /// <param name="age">Возраст по котором производится фильтрация</param>
        /// <returns></returns>
        [HttpGet("age/{age}")]
        public IActionResult GetUsers(string login, string password, int age)
        {
            BaseUserDTO userDto = new BaseUserDTO { RequestUser = new RequestUserDTO { Login = login, Password = password } };
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return Unauthorized("Incorrect request user data");

            if (!IsAdmin(reqUser!))
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied!");

            return Ok(_userService.GetUsersOlderAge(age));
        }

        /// <summary>
        /// Мягкое или полное удаление пользвателя
        /// </summary>
        /// <param name="userDto">Параметры запроса</param>
        /// <param name="userLogin">Логин пользователя</param>
        /// <returns></returns>
        [HttpDelete("{userLogin}")]
        public IActionResult DeleteUser(DeleteUserDTO userDto, string userLogin)
        {
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return Unauthorized("Incorrect request user data");

            if (!IsAdmin(reqUser!))
                return StatusCode(StatusCodes.Status403Forbidden, "Access denied!");

            var res = _userService.Delete(reqUser!.Login, userLogin, userDto.IsSoft);
            if(res.IsFound == false)
                return NotFound();

            if (res.IsSuccess == false)
                return BadRequest("Delete error");

            return Ok();
        }
        private (bool Res, string Msg, User? ReqUser, User? UpdateUser, int StatusCode) ValidateUpdateDTOAndCheckAccess<T>(T userDto, string login) where T: BaseUserDTO
        {
            if (!TryGetAuthor(userDto.RequestUser, out User? reqUser))
                return (false, "Incorrect request user data", null, null, StatusCodes.Status401Unauthorized);

            if (!IsAdmin(reqUser!) && !IsUserExistsAndActive(login, reqUser!))
                return (false, "Access denied!", null, null, StatusCodes.Status403Forbidden);

            User? updateUser = _userService.GetByLogin(login);
            if (updateUser is null)
                return (false, "The user being updated was not found", null, null, StatusCodes.Status404NotFound);

            return (true, "", reqUser, updateUser, 200);
        }

        private static bool IsUserExistsAndActive(string login, User reqUser) 
            => Equals(reqUser.Login, login) && reqUser.RevokedOn is null;

        private bool TryGetAuthor(RequestUserDTO? requestUser, out User? user)
        {
            user = _userService.GetByLoginPassword(requestUser!.Login!, requestUser.Password!);
            if (user is not null)
                return true;
            return false;
        }

        private static bool IsAdmin(User user)
            => user!.Admin;
    }
}
