using System.ComponentModel.DataAnnotations;

namespace RaceDay7.Models
{
    public class Weather
    {
        [Key]
        public int WeatherID { get; set; }
        public DateTime WeatherDate { get; set; }
        public decimal Temperature { get; set; }
        public string Conditions { get; set; }
        public int Humidity { get; set; }
        public decimal WindSpeed { get; set; }
        public string Notes { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}