using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cloudDevP1.Data;
using cloudDevP1.Models;

namespace cloudDevP1.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(string searchString)
        {
            var applicationDbContext = _context.Bookings.Include(b => b.Event).Include(b => b.Venue).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            { 
               applicationDbContext = applicationDbContext.Where(b => b.Venue.VenueName.Contains(searchString) || 
               b.Event.EventName.Contains(searchString));
            }
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
           
            var events = _context.Events.ToList();
            events.Insert(0, new Event { EventId = 0, EventName = "-- Select Event --" });

            var venues = _context.Venues.ToList();
            venues.Insert(0, new Venue { VenueId = 0, VenueName = "-- Select Venue --" });

            ViewData["EventId"] = new SelectList(events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingsId,BookingDate,VenueId,EventId")] Booking booking)
{
    // Basic validation for required fields
    if (booking.EventId == 0)
    {
        ModelState.AddModelError("EventId", "Please select an event");
    }
    
    if (booking.VenueId == 0)
    {
        ModelState.AddModelError("VenueId", "Please select a venue");
    }
    
    if (booking.BookingDate == default)
    {
        ModelState.AddModelError("BookingDate", "Please select a booking date");
    }

    // Only proceed with further checks if basic validation passes
    if (ModelState.IsValid)
    {
        var selectedEvent = await _context.Events.FirstOrDefaultAsync(e => e.EventId == booking.EventId);
        
        if (selectedEvent == null)
        {
            ModelState.AddModelError("EventId", "Selected event not found.");
        }
        else
        {
            // Check for double booking - same venue on the same date
            var conflict = await _context.Bookings
                .Include(b => b.Event)
                .AnyAsync(b => b.VenueId == booking.VenueId &&
                               b.BookingDate.Date == booking.BookingDate.Date);
            
            if (conflict)
            {
                ModelState.AddModelError("", "This venue is already booked for the selected date.");
            }
        }
    }

    // If everything is valid, try to save
    if (ModelState.IsValid)
    {
        try
        {
            _context.Add(booking);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Booking created successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                    // Log the exception (ex) here for debugging
                    ViewData["Events"] = _context.Events.ToList();
                    ViewData["Venues"] = _context.Venues.ToList();
                    return View(booking);
        }
    }

    // If we got here, something failed - repopulate dropdowns
    var events = _context.Events.ToList();
    events.Insert(0, new Event { EventId = 0, EventName = "-- Select Event --" });
    
    var venues = _context.Venues.ToList();
    venues.Insert(0, new Venue { VenueId = 0, VenueName = "-- Select Venue --" });
    
    ViewData["EventId"] = new SelectList(events, "EventId", "EventName", booking.EventId);
    ViewData["VenueId"] = new SelectList(venues, "VenueId", "VenueName", booking.VenueId);
    
    return View(booking);
}

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingsId,BookingDate,VenueId,EventId")] Booking booking)
        {
            if (id != booking.BookingsId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingsId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventId", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", booking.VenueId);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingsId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingsId == id);
        }
    }
}
