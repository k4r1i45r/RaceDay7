using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentID { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public string Status { get; set; }

        public int FK_ParticipantID { get; set; }
        public int FK_EventID { get; set; }

        [ForeignKey("FK_ParticipantID")] public Participant Participant { get; set; }
        [ForeignKey("FK_EventID")] public Event Event { get; set; }

        public Result Result { get; set; }
    }
}