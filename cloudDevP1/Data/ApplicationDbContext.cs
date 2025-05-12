using Microsoft.EntityFrameworkCore;
using cloudDevP1.Models;  

namespace cloudDevP1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .HasKey(e => e.EventId);

            modelBuilder.Entity<Venue>()
                .HasKey(v => v.VenueId);

            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("Bookings");
                entity.Property(e => e.BookingsId).HasColumnName("BookingsId"); // Map to DB column

                entity.HasOne(d => d.Event)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.EventId);

                entity.HasOne(d => d.Venue)
                    .WithMany(p => p.Bookings)
                    .HasForeignKey(d => d.VenueId);
            });
            

    }
}
}
