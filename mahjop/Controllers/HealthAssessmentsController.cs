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
using mahjop.Migrations;


namespace mahjop.Controllers
{
    public class HealthAssessmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public HealthAssessmentsController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

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

            // قم بتحميل البيانات الأخرى كما فعلت في السابق
            ViewBag.PatientId = id;
            var applicationDbContext = _context.HealthAssessments.Include(m => m.Patient).Where(p => p.PatientId == id);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: HealthAssessments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthAssessment = await _context.HealthAssessments
                .Include(h => h.Patient)
                .FirstOrDefaultAsync(m => m.HealthAssessmentId == id);
            if (healthAssessment == null)
            {
                return NotFound();
            }

            return View(healthAssessment);
        }

        // GET: HealthAssessments/Create
        public IActionResult Create(int patientId)
        {
            ViewData["PatientId"] = patientId;
            return View();
        }

        // POST: HealthAssessments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HealthAssessmentId,AssessmentDate,BloodPressure,BloodSugarLevel,Weight,PatientId")] HealthAssessment healthAssessment)
        {
            if (ModelState.IsValid)
            {
                healthAssessment.AssessmentDate = DateTime.Now;
                _context.Add(healthAssessment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "HealthAssessments", new { id = healthAssessment.PatientId });
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", healthAssessment.PatientId);
            return View(healthAssessment);
        }

        // GET: HealthAssessments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthAssessment = await _context.HealthAssessments.FindAsync(id);
            if (healthAssessment == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", healthAssessment.PatientId);
            return View(healthAssessment);
        }

        // POST: HealthAssessments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HealthAssessmentId,AssessmentDate,BloodPressure,BloodSugarLevel,Weight,PatientId")] HealthAssessment healthAssessment)
        {
            if (id != healthAssessment.HealthAssessmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    healthAssessment.AssessmentDate = DateTime.Now;
                    _context.Update(healthAssessment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthAssessmentExists(healthAssessment.HealthAssessmentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "HealthAssessments", new { id = healthAssessment.PatientId });
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", healthAssessment.PatientId);
            return View(healthAssessment);
        }

        // GET: HealthAssessments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthAssessment = await _context.HealthAssessments
                .Include(h => h.Patient)
                .FirstOrDefaultAsync(m => m.HealthAssessmentId == id);
            if (healthAssessment == null)
            {
                return NotFound();
            }

            return View(healthAssessment);
        }

        // POST: HealthAssessments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var healthAssessment = await _context.HealthAssessments.FindAsync(id);
            _context.HealthAssessments.Remove(healthAssessment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "HealthAssessments", new { id = healthAssessment.PatientId });
        }

        private bool HealthAssessmentExists(int id)
        {
            return _context.HealthAssessments.Any(e => e.HealthAssessmentId == id);
        }
    }
}
