using System.ComponentModel.DataAnnotations;

namespace AtonTestTask.Web.DTOModels
{
    public class UpdateUserInfoDTO: UpdateUserDTO
    {
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Za-яёА-ЯЁ]+$")]
        public string? Name { get; set; } = null;
        [Range(0, 2)]
        public int? Gender { get; set; } = null;
        public DateTime? Birthday { get; set; } = null;
    }
}
