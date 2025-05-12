using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cloudDevP1.Data;
using cloudDevP1.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http; // For IFormFile
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace cloudDevP1.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VenueId,VenueName,Location,Capacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (ModelState.IsValid)
            {
                if (venue.ImageFile != null)
                {

                    
                    var blobUrl = await UploadImageToBlobAsync(venue.ImageFile); 

                    venue.ImageUrl = blobUrl;
                }

                _context.Add(venue);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "venue created successfully. ";
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        // POST: Venues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VenueId,VenueName,Location,Capacity,ImageUrl,ImageFile")] Venue venue)
        {
            if (id != venue.VenueId) return NotFound();
            

            if (ModelState.IsValid)
            {
                try
                {
                    if (venue.ImageFile != null)
                    {
                        
                        var blobUrl = await UploadImageToBlobAsync(venue.ImageFile);

                        
                        venue.ImageUrl = blobUrl;
                    }
                    else
                    {
                        // Keep the existing ImageUrl 
                    }

                    _context.Update(venue);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Venue updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venue.VenueId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null)
            {
                return NotFound();
            }

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound(); 

            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "Cannot delete venue because it has existing bookings. ";
                return RedirectToActionPermanent(nameof(Index));
             }

            
            _context.Venues.Remove(venue);            
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Venue deleted Successfully.";
            return RedirectToAction(nameof(Index));

        }

        private async Task<string> UploadImageToBlobAsync(IFormFile imageFile)
        {
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=cloudpoe2025;AccountKey=VTD4J8WOZcrqPFscXrTIKvU4qJYFU138yycwqPtDbT0E8BTDnTMnpQEdmuj4rluwCwDrTkaqs3Yn+AStkXrV0g==;EndpointSuffix=core.windows.net";
            var containerName = "cldv6211p2";

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid() + Path.GetExtension(imageFile.FileName));

            var blobHttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = imageFile.ContentType
            };

            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }
        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}
