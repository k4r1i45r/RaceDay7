using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }
        public string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public TimeSpan EventTime { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public int MaxParticipants { get; set; }

        public int? FK_CategoryID { get; set; }
        public int? FK_OrganizerID { get; set; }
        public int? FK_RouteID { get; set; }
        public int? FK_WeatherID { get; set; }

        [ForeignKey("FK_CategoryID")] public Category Category { get; set; }
        [ForeignKey("FK_OrganizerID")] public EventPlanner Organizer { get; set; }

        // FIXED: Added DemoProject.Models. to distinguish it from ASP.NET's built-in Route
        [ForeignKey("FK_RouteID")] public RaceDay7.Models.Route Route { get; set; }

        [ForeignKey("FK_WeatherID")] public Weather Weather { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
