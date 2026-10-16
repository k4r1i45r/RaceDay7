using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }
        public TimeSpan FinishTime { get; set; }
        public TimeSpan ChipTime { get; set; }
        public int OverallPosition { get; set; }
        public int CategoryPosition { get; set; }
        public string Pace { get; set; }
        public string Status { get; set; }

        public int FK_EnrollmentID { get; set; }

        [ForeignKey("FK_EnrollmentID")]
        public Enrollment Enrollment { get; set; }
    }
}