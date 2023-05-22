using System.ComponentModel.DataAnnotations;

namespace AtonTestTask.Core.Models
{
    public class User: ICloneable
    {
        [Key]
        public Guid Id { get; set; }

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

        public DateTime? Birthday { get; set; } = null;

        [Required]
        public bool Admin { get; set;}

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        public string CreatedBy { get; set; } = null!;

        public DateTime? ModifiedOn { get; set; } = null;

        [MaxLength(100)]
        public string? ModifiedBy { get; set; } = null;
        public DateTime? RevokedOn { get; set; } = null;

        [MaxLength(100)]
        public string? RevokedBy { get; set; } = null;

        public object Clone() 
            => new User
            {
                Id = Id,
                Login = Login,
                Password = Password,
                Name = Name,
                Gender = Gender,
                Birthday = Birthday,
                Admin = Admin,
                CreatedOn = CreatedOn,
                CreatedBy = CreatedBy,
                ModifiedOn = ModifiedOn,
                ModifiedBy = ModifiedBy,
                RevokedOn = RevokedOn,
                RevokedBy = RevokedBy,
            };

        public int GetAge()
        {
            if (Birthday is not null)
            {
                DateTime bday = (DateTime)Birthday;
                var today = DateTime.Today;

                var todayFromated = (today.Year * 100 + today.Month) * 100 + today.Day; // to format yyyyMMdd
                var birthdayFormated = (bday.Year * 100 + bday.Month) * 100 + bday.Day;

                return (todayFromated - birthdayFormated) / 10000;
            }

            return 0;
        }

        public void Copy(User user)
        {
            Id = user.Id;
            Login = user.Login;
            Password = user.Password;
            Name = user.Name;
            Gender = user.Gender;
            Birthday = user.Birthday;
            Admin = user.Admin;
            CreatedOn = user.CreatedOn;
            CreatedBy = user.CreatedBy;
            ModifiedOn = user.ModifiedOn;
            ModifiedBy = user.ModifiedBy;
            RevokedOn = user.RevokedOn;
            RevokedBy = user.RevokedBy;
        }
    }
}
