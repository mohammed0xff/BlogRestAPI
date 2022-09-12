using System.ComponentModel.DataAnnotations;

namespace Models.ApiModels
{
    public class LoginModelRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
