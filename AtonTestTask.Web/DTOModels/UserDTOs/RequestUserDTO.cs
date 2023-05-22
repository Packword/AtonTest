using System.ComponentModel.DataAnnotations;

namespace AtonTestTask.Web.DTOModels
{
    public class RequestUserDTO
    {
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; } = null!;
    }
}
