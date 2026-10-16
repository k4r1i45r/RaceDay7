using RaceDay7.Models;
using System.ComponentModel.DataAnnotations;

public class Participant
{
    [Key]
    public int ParticipantID { get; set; }
    public string UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string Province { get; set; }
    public string EmergencyContactName { get; set; }
    public string EmergencyContactNumber { get; set; }

    // NEW: Add this for the profile picture
    public string? ProfileImagePath { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
}
