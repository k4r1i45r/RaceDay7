using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
    public class EventPlanner
    {
        [Key]
        public int OrganizerID { get; set; }
        public string UserId { get; set; } 
        public string OrganizerName { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Navigation property
        public ICollection<Event> Events { get; set; }
    }
}