using System.ComponentModel.DataAnnotations;

namespace AtonTestTask.Web.DTOModels
{
    public class CreateUserDTO: BaseUserDTO
    {
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Login { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string Password { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Za-яёА-ЯЁ]+$")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 2)]
        public int Gender { get; set; } //0 - Female, 1 - Male, 2 - Uknown

        [Required]
        public bool Admin { get; set; }

        public DateTime? Birthday { get; set; } = null;

    }
}
