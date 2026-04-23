using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordStudioGamzaev.Models;

namespace RecordStudioGamzaev.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Bookings.Include(b => b.Studio);
            return View(await _context.Bookings
                    .Include(b => b.Studio)
                        .ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Studio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,StudioId,StartTime,EndTime")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // проверки времени
                if (booking.EndTime <= booking.StartTime)
                {
                    ModelState.AddModelError("", "Время окончания должно быть больше начала");
                    ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name", booking.StudioId);
                    return View(booking);
                }

                // проверка занятости
                var isBusy = _context.Bookings.Any(b =>
                    b.StudioId == booking.StudioId &&
                    booking.StartTime < b.EndTime &&
                    booking.EndTime > b.StartTime);

                if (isBusy)
                {
                    ModelState.AddModelError("", "Это время уже занято!");
                    ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name", booking.StudioId);
                    return View(booking);
                }

                // расчёт цены
                var hours = (booking.EndTime - booking.StartTime).TotalHours;
                var studio = _context.Studios.First(s => s.Id == booking.StudioId);
                booking.Price = (decimal)hours * studio.PricePerHour;

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ✅ ВОТ ЭТО ДОБАВЬ (самое главное)
            ViewBag.StudioId = new SelectList(_context.Studios, "Id", "Name", booking.StudioId);
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
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name", booking.StudioId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudioId,StartTime,EndTime")] Booking booking)
        {
            if (id != booking.Id)
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
                    if (!BookingExists(booking.Id))
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
            ViewData["StudioId"] = new SelectList(_context.Studios, "Id", "Name", booking.StudioId);
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
                .Include(b => b.Studio)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            return _context.Bookings.Any(e => e.Id == id);
        }
    }
}
