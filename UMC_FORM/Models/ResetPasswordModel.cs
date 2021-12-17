using System.ComponentModel.DataAnnotations;
namespace UMC_FORM.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string password { get; set; }
        [Required]
        public string confirmPassword { get; set; }
        public string returnToken { get; set; }
    }
}
