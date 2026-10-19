using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
    // THIS IS THE MISSING PIECE
    public enum Gender
    {
        Male,
        Female,
        Other
    }

    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Gender Gender { get; set; }
        public string? ProfileImagePath { get; set; }

        // 1-to-1 Role setup
        public string? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public IdentityRole? Role { get; set; }
    }
}