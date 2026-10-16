using System.ComponentModel.DataAnnotations;

namespace RaceDay7.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string CategoryType { get; set; } // e.g., "Age" or "Distance"
        public string Description { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
