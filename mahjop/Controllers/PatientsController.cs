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
using mahjop.Migrations;
using System.IO;

namespace mahjop.Controllers
{
    public class PatientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PatientsController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        // GET: Patients
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin") || roles.Contains("Lab") || roles.Contains("Doctor") || roles.Contains("pharmacy"))
                {
                    // إذا كان المستخدم غير مسجل دخوله ولديه أحد الأدوار المسموح بها
                    var applicationDbContext = _context.Patients;
                    return View(await applicationDbContext.ToListAsync());
                }
                else
                {
                    // إذا كان المستخدم مسجل دخوله ولكن ليس لديه أحد الأدوار المسموح بها
                    var userId = user.Id;
                    var applicationDbContext = _context.Patients;
                    return View(await applicationDbContext.Where(o => o.UserId == userId).ToListAsync());
                }
            }

            // إذا لم يكن المستخدم مسجل دخوله
            return View();
        }


        // GET: Patients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patients/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Patients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FullName,DateOfBirth,Gender,Address,PhoneNumber,Weight,Height,UserId")] Patient patient)
        {

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                if (ModelState.IsValid)
                {
                    // التحقق من عدم وجود حساب مريض مسبقًا لنفس المستخدم
                    if (!_context.Patients.Any(d => d.UserId == userId))
                    {
                        // التحقق من عدم وجود اسم صيدلية مكرر
                        if (!_context.Labs.Any(d => d.Name == patient.FullName))
                        {
                            
                            // إضافة الصيدلية إلى قاعدة البيانات
                            patient.UserId = userId;
                          
                            _context.Add(patient);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // رسالة الخطأ في حالة وجود اسم المختبرات مكرر
                            ModelState.AddModelError("", "This lab name already exists.");
                        }
                    }
                    else
                    {
                        // رسالة الخطأ في حالة وجود حساب المختبرات بالفعل
                        ModelState.AddModelError("", "You have already created a lab account.");
                    }
                }
            }

            return View(patient);
        }


        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }

        // POST: Patients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PatientId,FullName,DateOfBirth,Gender,Address,PhoneNumber,Weight,Height,UserId")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
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
            return View(patient);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePatients(int? id)
        {
            if (id == null)
            {
                return BadRequest(); // التعامل مع حالة الـ id == null بشكل صحيح
            }

            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound(); // التعامل مع عدم وجود الـ test بالـ id المعطى
            }

            _context.Patients.Remove(patient); // إزالة الـ test من السياق

            var result = await _context.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات

            if (result > 0)
            {
                return Json(new { success = true }); // تمت عملية الحذف بنجاح
            }

            return Json(new { success = false }); // فشلت عملية الحذف
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
