using System.ComponentModel.DataAnnotations;
namespace cloudDevP1.Models
{
    public class Event
    {
        [Key] public int EventId { get; set; }
        [Required] public string? EventName { get; set; }
        [Required][DataType(DataType.Date)] public DateTime EventDate { get; set; }
        [Required] public string? Description { get; set; }
        [Required] public int VenueId { get; set; }
        public Venue? Venue { get; set; }
        public List<Booking> Bookings { get; set; } = new();
    }
}