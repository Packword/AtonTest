using System.ComponentModel.DataAnnotations;

namespace AtonTestTask.Web.DTOModels
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class BaseUserDTO
    {
        [Required]
        public RequestUserDTO RequestUser { get; set; } = null!;
    }
}
