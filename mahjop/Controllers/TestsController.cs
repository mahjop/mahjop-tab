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
using mahjop.ViewModels;

namespace mahjop.Controllers
{
    public class TestsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TestsController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
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
            TestViewModel model = new TestViewModel
            {
                Tests = await _context.Tests
                                .Include(a => a.Patient)
                                .Where(a => a.Patient.PatientId == id)
                                .ToListAsync()
            };

            // Return the view with the populated model
            return View(model);
        }


        // GET: Tests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .Include(t => t.Patient)
                .FirstOrDefaultAsync(m => m.TestId == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

       
        // GET: Tests/Create
        public IActionResult Create(int patientId)
        {
            ViewData["PatientId"] = patientId;
            return View();
        }

        // POST: Tests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TestId,Name,Result,PatientId")] Test test)
        {
            if (ModelState.IsValid)
            {
                _context.Add(test);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Tests", new { id = test.PatientId }); // Redirect to the patient details page
            }
            return View(test);
        }



        // GET: Tests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", test.PatientId);
            return View(test);
        }

        // POST: Tests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TestId,Name,Result,PatientId")] Test test)
        {
            if (id != test.TestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(test);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(test.TestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Tests", new { id = test.PatientId });
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", test.PatientId);
            return View(test);
        }

       

        [HttpDelete]
        public async Task<IActionResult> DeleteTest(int? id)
        {
            if (id == null)
            {
                return BadRequest(); // التعامل مع حالة الـ id == null بشكل صحيح
            }

            var test = await _context.Tests.FindAsync(id);

            if (test == null)
            {
                return NotFound(); // التعامل مع عدم وجود الـ test بالـ id المعطى
            }

            _context.Tests.Remove(test); // إزالة الـ test من السياق

            var result = await _context.SaveChangesAsync(); // حفظ التغييرات في قاعدة البيانات

            if (result > 0)
            {
                return Json(new { success = true }); // تمت عملية الحذف بنجاح
            }

            return Json(new { success = false }); // فشلت عملية الحذف
        }



        private bool TestExists(int id)
        {
            return _context.Tests.Any(e => e.TestId == id);
        }
    }
}
