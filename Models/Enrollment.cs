using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay7.Models;

// Links Participant + Event + Category
public class Enrolment
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    [ForeignKey("Participant")]
    public string ParticipantId { get; set; } = string.Empty;
    public ApplicationUser? Participant { get; set; }

    public DateTime EnrolledOn { get; set; } = DateTime.UtcNow;

    public Result? Result { get; set; }
}