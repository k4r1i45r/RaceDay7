using System.ComponentModel.DataAnnotations;

namespace RaceDay7.Models
{
    public class Route
    {
        [Key]
        public int RouteID { get; set; }
        public string RouteName { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public decimal Distance { get; set; }
        public decimal Elevation { get; set; }
        public string Description { get; set; }
    }
}