using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
    public enum Gender
    {
        Male,
        Female,
        Other,
        PreferNotToSay
    }

    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public Gender Gender { get; set; }

        // Nullable because they might not upload an image
        public string? ProfileImagePath { get; set; }

        // Calculated property for the UI to display both names at once
        public string FullName => $"{FirstName} {LastName}";

        public string? RoleId { get; set; }

        [ForeignKey("RoleId")]
        public IdentityRole? Role { get; set; }
    }
}
