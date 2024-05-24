using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahjop.Data;
using mahjop.Models;
using mahjop.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace mahjop.Controllers
{
    public class MedicationsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public MedicationsController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Tests
        public async Task<IActionResult> Index(int? id)
        {
            // Get current user id
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
            // Initialize the view model
            MedicationsViewModel model = new MedicationsViewModel
            {
                Medications = await _context.Medications.Include(m=>m.MedicationCategory)
                                .Include(a => a.Patient)
                                .Where(a => a.Patient.PatientId == id)
                                .ToListAsync()
            };

            // Return the view with the populated model
            return View(model);
        }
        

        // GET: Medications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications
                .Include(m => m.MedicationCategory)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicationId == id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }
        // GET: Medications/Create
        public IActionResult Create(int patientId)
        {
            ViewData["PatientId"] = patientId;
            ViewData["MedicationCategoryId"] = new SelectList(_context.MedicationCategories, "MedicationCategoryId", "Name");

            return View();
        }

        // POST: Medications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicationId,Name,Description,DosageFrequency,MedicationCategoryId,PatientId")] Medication medication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medication);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Medications", new { id = medication.PatientId }); // Redirect to the patient details page
            }
            ViewData["MedicationCategoryId"] = new SelectList(_context.MedicationCategories, "MedicationCategoryId", "Name", medication.MedicationCategoryId);
            return View(medication);
        }


        // GET: Medications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["MedicationCategoryId"] = new SelectList(_context.MedicationCategories, "MedicationCategoryId", "MedicationCategoryId", medication.MedicationCategoryId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", medication.PatientId);
            return View(medication);
        }

        // POST: Medications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicationId,Name,Description,DosageFrequency,MedicationCategoryId,PatientId,UserId")] Medication medication)
        {
            if (id != medication.MedicationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.MedicationId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Medications", new { id = medication.PatientId }); // Redirect to the patient details page
            }
            ViewData["MedicationCategoryId"] = new SelectList(_context.MedicationCategories, "MedicationCategoryId", "MedicationCategoryId", medication.MedicationCategoryId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", medication.PatientId);
            return View(medication);
        }

        // GET: Medications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.Medications
                .Include(m => m.MedicationCategory)
                .Include(m => m.Patient)
                .FirstOrDefaultAsync(m => m.MedicationId == id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // POST: Medications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medication = await _context.Medications.FindAsync(id);
            _context.Medications.Remove(medication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(int id)
        {
            return _context.Medications.Any(e => e.MedicationId == id);
        }
    }
}
