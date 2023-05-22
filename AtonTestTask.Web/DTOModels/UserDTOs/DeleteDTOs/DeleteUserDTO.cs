using System.ComponentModel.DataAnnotations;

namespace AtonTestTask.Web.DTOModels
{
    public class DeleteUserDTO: BaseUserDTO
    {
        [Required]
        public bool IsSoft { get; set; }
    }
}
