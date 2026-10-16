using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public IdentityRole? Role { get; set; }
    }
}
