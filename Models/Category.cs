using System.ComponentModel.DataAnnotations;

namespace RaceDay7.Models;

public enum CategoryKind
{
    Age,
    Distance
}

public class Category
{
    public int Id { get; set; }

    public int EventId { get; set; }
    public Event? Event { get; set; }

    [Required, StringLength(100)]
    [Display(Name = "Category name")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Category type")]
    public CategoryKind Kind { get; set; }

    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}