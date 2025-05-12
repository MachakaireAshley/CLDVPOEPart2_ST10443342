using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cloudDevP1.Models
{
    public class Booking
    {
        [Key]
        [Column("BookingsId")]
        public int BookingsId { get; set; }
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }
        public int VenueId { get; set; }
        public int EventId { get; set; }

        [ForeignKey("VenueId")]
        public Venue? Venue { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }
    }
}