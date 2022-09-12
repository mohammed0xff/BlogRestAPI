using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }
        public ICollection<Blog> Blogs { get; set; }
    }


    public static class UserRoles   
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

}




