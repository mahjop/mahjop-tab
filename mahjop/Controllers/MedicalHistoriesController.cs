using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahjop.Data;
using mahjop.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.IO;

namespace mahjop.Controllers
{
    public class MedicalHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public MedicalHistoriesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: MedicalHistories
        public async Task<IActionResult> Index(int? id)

        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // ابحث عن المستخدم باستخدام معرفه
            var user = await _userManager.FindByIdAsync(currentUserId);

            // تحقق من أن المستخدم موجود
            if (user != null)
            {
                // استخدم اسم المستخدم في ViewBag لاستخدامه في العرض
                ViewBag.PatientName = user.UserName;
            }
            else
            {
                // في حالة عدم العثور على المستخدم، يمكنك تعيين قيمة افتراضية أو إرسال رسالة خطأ
                ViewBag.PatientName = "User Not Found";
            }
            ViewBag.PatientId = id;
            var applicationDbContext = _context.MedicalHistories.Include(m => m.Patient).Where(p=>p.PatientId==id);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: MedicalHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalHistory = await _context.MedicalHistories
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalHistoryId == id);
            if (medicalHistory == null)
            {
                return NotFound();
            }

            return View(medicalHistory);
        }

        // GET: MedicalHistories/Create
        public IActionResult Create(int patientId)
        {
            ViewData["PatientId"] = patientId;
            return View();
        }

        // POST: MedicalHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicalHistoryId,Date,Diagnosis,Notes,DoctorName,Hospital,PatientId")] MedicalHistory medicalHistory)
        {
            if (ModelState.IsValid)
            {
                medicalHistory.Date = DateTime.Now; // هنا يتم تعيين الوقت الحالي
                _context.Add(medicalHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index","MedicalHistories",new {id=medicalHistory.PatientId});
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", medicalHistory.PatientId);
            return View(medicalHistory);
        }


        // GET: MedicalHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalHistory = await _context.MedicalHistories.FindAsync(id);
            if (medicalHistory == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", medicalHistory.PatientId);
            return View(medicalHistory);
        }

        // POST: MedicalHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicalHistoryId,Date,Diagnosis,Notes,DoctorName,Hospital,PatientId")] MedicalHistory medicalHistory)
        {
            if (id != medicalHistory.MedicalHistoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    medicalHistory.Date = DateTime.Now; // هنا يتم تعيين الوقت الحالي
                    _context.Update(medicalHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalHistoryExists(medicalHistory.MedicalHistoryId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","MedicalHistories", new { id = medicalHistory.PatientId });
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", medicalHistory.PatientId);
            return View(medicalHistory);
        }

        // GET: MedicalHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalHistory = await _context.MedicalHistories
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicalHistoryId == id);
            if (medicalHistory == null)
            {
                return NotFound();
            }

            return View(medicalHistory);
        }

        // POST: MedicalHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalHistory = await _context.MedicalHistories.FindAsync(id);
            _context.MedicalHistories.Remove(medicalHistory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","MedicalHistories", new { id = medicalHistory.PatientId });
        }

        private bool MedicalHistoryExists(int id)
        {
            return _context.MedicalHistories.Any(e => e.MedicalHistoryId == id);
        }
    }
}
