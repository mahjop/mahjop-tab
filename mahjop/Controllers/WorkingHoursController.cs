using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahjop.Data;
using mahjop.Models;

namespace mahjop.Controllers
{
    public class WorkingHoursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WorkingHoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WorkingHours
        public async Task<IActionResult> Index(int id)
        {
            // استعلام للعثور على أول طبيب يطابق DoctorId الممررة
            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.DoctorId == id);
            ViewBag.doctorId = id;
            if (doctor != null)
            {
                // استعلام للحصول على ساعات العمل للطبيب المحدد
                var workingHours = await _context.WorkingHours
                    .Where(wh => wh.DoctorId == id)
                    .ToListAsync();

                return View(workingHours);
            }
            else
            {
                // إرسال استجابة خاصة بالخطأ في حالة عدم العثور على الطبيب
                return NotFound();
            }
        }


        // GET: WorkingHours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHours = await _context.WorkingHours
                .Include(w => w.Doctor)
                .FirstOrDefaultAsync(m => m.WorkingHoursId == id);
            if (workingHours == null)
            {
                return NotFound();
            }

            return View(workingHours);
        }

        // GET: WorkingHours/Create
        public IActionResult Create(int doctorId)
        {
            ViewData["DoctorId"] = doctorId;
            return View();
        }

        // POST: WorkingHours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkingHoursId,Day,StartTime,EndTime,DoctorId")] WorkingHours workingHours)
        {
            if (ModelState.IsValid)
            {
                _context.Add(workingHours);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "WorkingHours", new {id=workingHours.DoctorId});
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", workingHours.DoctorId);
            return View(workingHours);
        }

        // GET: WorkingHours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHours = await _context.WorkingHours.FindAsync(id);
            if (workingHours == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", workingHours.DoctorId);
            return View(workingHours);
        }

        // POST: WorkingHours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkingHoursId,Day,StartTime,EndTime,DoctorId")] WorkingHours workingHours)
        {
            if (id != workingHours.WorkingHoursId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workingHours);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkingHoursExists(workingHours.WorkingHoursId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "WorkingHours", new { id = workingHours.DoctorId });
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", workingHours.DoctorId);
            return View(workingHours);
        }

        // GET: WorkingHours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workingHours = await _context.WorkingHours
                .Include(w => w.Doctor)
                .FirstOrDefaultAsync(m => m.WorkingHoursId == id);
            if (workingHours == null)
            {
                return NotFound();
            }

            return View(workingHours);
        }

        // POST: WorkingHours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var workingHours = await _context.WorkingHours.FindAsync(id);
            _context.WorkingHours.Remove(workingHours);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "WorkingHours", new { id = workingHours.DoctorId });
        }

        private bool WorkingHoursExists(int id)
        {
            return _context.WorkingHours.Any(e => e.WorkingHoursId == id);
        }
    }
}
