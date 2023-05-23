using System.ComponentModel.DataAnnotations;

namespace AtonTestTask.Web.DTOModels
{
    public class UpdateUserLoginDTO: BaseUserDTO
    {
        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string NewLogin { get; set; } = null!;
    }
}
