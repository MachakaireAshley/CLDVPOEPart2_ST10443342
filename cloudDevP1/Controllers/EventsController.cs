﻿using System;
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
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Events.Include(a => a.Venue);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(a => a.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            var venues = _context.Venues.ToList();
            venues.Insert(0, new Venue { VenueId = 0, VenueName = "-- Select Venue --" });
            ViewData["VenueId"] = new SelectList(venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            if (@event.VenueId == 0)
            {
                ModelState.AddModelError("VenueId", "Please select a venue");
            }

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var venues = _context.Venues.ToList();
            venues.Insert(0, new Venue { VenueId = 0, VenueName = "-- Select Venue --" });
            ViewData["VenueId"] = new SelectList(venues, "VenueId", "VenueName", @event.VenueId);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,Description,VenueId")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
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
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueId", @event.VenueId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(a => a.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            var isBooked = await _context.Bookings.AnyAsync(b => b.EventId == id);
            if (isBooked)
            {
                TempData["ErrorMessage"] = "Cannot delete event because it has existing bookings. ";
                return RedirectToActionPermanent(nameof(Index));
            }


            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Event deleted Successfully.";
            return RedirectToAction(nameof(Index));
        }
        
            

         

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }

}
