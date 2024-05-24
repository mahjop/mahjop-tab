using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahjop.Data;
using mahjop.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace mahjop.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AppointmentsController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var role = await _roleManager.FindByNameAsync("Admin");
            if (role != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // إذا كان المستخدم غير مسجل دخوله ولديه دور "Admin"
                var applicationDbContext = _context.Appointment.Include(a => a.Doctor);
                return View(await applicationDbContext.ToListAsync());
            }

            if (user != null)
            {
                // إذا كان المستخدم مسجل دخوله
                var userId = user.Id;
                var applicationDbContext = _context.Appointment.Include(a => a.Doctor);
                return View(await applicationDbContext.Where(a => a.UserId == userId).ToListAsync());
            }

            return View();
        }

        public async Task<IActionResult> IndexDoctor(int id)
        {
            // Get current user id
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve appointments where doctor's id matches and doctor's UserId matches current user's UserId
            var appointments = _context.Appointment
                                        .Include(a => a.Doctor)
                                        .Where(a => a.Doctor.DoctorId == id);

            return View(await appointments.ToListAsync());
        }



        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create(int doctorId)
        {
            // Get current user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create new Appointment object with UserId set
            var appointment = new Appointment { UserId = userId };

            // Populate ViewData with list of doctors
            ViewData["DoctorId"] = doctorId;

            return View(appointment);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,AppointmentTime,AppointmentDay,IsConfirmed,DoctorId,UserId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "NameDoctor", appointment.DoctorId);
            return View(appointment);
        }


        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", appointment.DoctorId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AppointmentId,AppointmentTime,AppointmentDay,IsConfirmed,DoctorId,UserId")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", appointment.DoctorId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointment
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointment.FindAsync(id);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.AppointmentId == id);
        }
    }
}
