using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
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

        // Navigation property for Enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
